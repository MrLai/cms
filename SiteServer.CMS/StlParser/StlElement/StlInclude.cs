﻿using System.Collections.Generic;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "包含文件", Description = "通过 stl:include 标签在模板中包含另一个文件，作为模板的一部分")]
    public class StlInclude
	{
		private StlInclude(){}
		public const string ElementName = "stl:include";

        private static readonly Attr File = new Attr("file", "文件路径");
        
        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var file = string.Empty;
            var parameters = new Dictionary<string, string>();

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, File.Name))
                {
                    file = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    file = PageUtility.AddVirtualToUrl(file);
                }
                else
                {
                    parameters[name] = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            return ParseImpl(pageInfo, contextInfo, file, parameters);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string file, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;

            var pageParameters = pageInfo.Parameters;
            pageInfo.Parameters = parameters;

            var content = TemplateManager.GetIncludeContent(pageInfo.SiteInfo, file, pageInfo.TemplateInfo.Charset);
            content = StlParserUtility.Amp(content);
            var contentBuilder = new StringBuilder(content);
            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            var parsedContent = contentBuilder.ToString();

            pageInfo.Parameters = pageParameters;

            return parsedContent;
        }
	}
}