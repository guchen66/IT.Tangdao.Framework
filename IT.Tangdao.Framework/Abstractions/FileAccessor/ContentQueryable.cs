using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Selectors;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using IT.Tangdao.Framework.Infrastructure.Ambient;
using IT.Tangdao.Framework.Threading;
using System.Linq;
using System.Xml.Linq;

namespace IT.Tangdao.Framework.Abstractions
{
    internal class ContentQueryable : IContentQueryable
    {
        public int ReadIndex
        {
            get => _readIndex;
            set => _readIndex = value;
        }

        private int _readIndex = -1;

        // 实现接口中的索引器
        public IContentQueryable this[int readIndex]
        {
            get
            {
                _readIndex = readIndex;
                return this;
            }
        }

        private DaoFileType _selectedType = DaoFileType.None;
        private string content;
        private bool isReaded;//标志位，是否已经读取

        public IContentQueryable Auto()
        {
            if (isReaded)
            {
                _selectedType = FileSelector.DetectFromContent(content);
            }
            return this;
        }

        public IContentQueryable AsXml()
        {
            _selectedType = DaoFileType.Xml;
            return this;
        }

        public IContentQueryable AsJson()
        {
            _selectedType = DaoFileType.Json;
            return this;
        }

        public IContentQueryable AsConfig()
        {
            _selectedType = DaoFileType.Config;
            return this;
        }

        // -------- 查询方法（暂未实现） --------

        public ReadResult<T> Select<T>(string key = null)
        {
            if (_selectedType == DaoFileType.None)
                return ReadResult<T>.Failure("未指定或自动探测格式");

            return ReadResult<T>.Failure($"[{_selectedType}] 解析未实现");
        }

        public ReadResult<List<T>> SelectNodes<T>() where T : new()
        {
            if (_selectedType == DaoFileType.None)
                return ReadResult<List<T>>.Failure("未指定或自动探测格式");

            return ReadResult<List<T>>.Failure($"[{_selectedType}] 解析未实现");
        }

        // ContentQueryable 内新增
        public IContentQueryable Read(string path, DaoFileType daoFileType = DaoFileType.None)
        {
            // string content = string.Empty;

            if (daoFileType == DaoFileType.None)
            {
                daoFileType = DaoFileType.Txt;
            }
            content = path.UseStreamReadToEnd();
            AmbientContext.SetCurrent(AmbientKeys.Content, content);
            isReaded = true;
            return this;
        }

        public ReadResult SelectNode(string node)
        {
            try
            {
                string content = AmbientContext.GetCurrent<string>(AmbientKeys.Content);
                AmbientContext.GetCurrent<string>(AmbientKeys.FileType);
                var doc = XDocument.Parse(content);
                var root = doc.RootElement();
                var xmlType = FileSelector.DetectXmlStructure(doc);

                switch (xmlType)
                {
                    case DaoXmlType.Empty:
                        return ReadResult.Failure("XML内容为空");

                    case DaoXmlType.None:
                        return ReadResult.Failure("XML只有声明没有内容");

                    case DaoXmlType.Single:
                        // 单节点结构 - 直接查找目标节点
                        var singleElement = root.Element(node) ?? root.Elements().First().Element(node);
                        if (singleElement == null)
                        {
                            return ReadResult.Failure($"节点 '{node}' 不存在");
                        }
                        return ReadResult.Success(singleElement.Value);

                    case DaoXmlType.Multiple:
                        if (ReadIndex < 0)
                        {
                            // 尝试直接查找节点(适用于扁平结构)
                            var directElement = root.Element(node);
                            if (directElement != null)
                            {
                                return ReadResult.Success(directElement.Value);
                            }
                            return ReadResult.Failure("存在多个节点，请指定索引");
                        }
                        else
                        {
                            // 处理指定索引的情况
                            var parentNode = root.Elements().ElementAtOrDefault(ReadIndex);
                            if (parentNode == null)
                            {
                                return ReadResult.Failure($"索引 {ReadIndex} 超出范围");
                            }

                            var targetElement = parentNode.Element(node);
                            if (targetElement == null)
                            {
                                return ReadResult.Failure($"子节点 '{node}' 不存在");
                            }

                            return ReadResult.Success(targetElement.Value);
                        }

                    default:
                        return ReadResult.Failure("未知的XML结构类型");
                }
            }
            catch (Exception ex)
            {
                return ReadResult.Failure($"XML解析错误: {ex.Message}");
            }
            finally
            {
                AmbientContext.ClearCurrent<string>(AmbientKeys.Content);
                AmbientContext.ClearCurrent<object>(AmbientKeys.FileType);
            }
        }
    }
}