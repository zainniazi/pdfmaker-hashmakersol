using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace hashmakersol.pdfmaker
{
    public static class Constants
    {
        private const string CssPattern = @"<link(.|\n)+?\/>";
        private const string ImagePattern = @"<img(.|\n)+?>";

        /// <summary>
        ///     Convert relative link to absolute link
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FormatImageLinks(this string html)
        {
            if (html == null)
                return string.Empty;
            var tempInput = html;
            var context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (
                Match m in
                Regex.Matches(html, ImagePattern,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft))
                if (m.Success)
                {
                    var tempM = m.Value;
                    var pattern1 = "src=[\'|\"](.+?)[\'|\"]";
                    var reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    var mImg = reImg.Match(m.Value);

                    if (mImg.Success)
                    {
                        var src = mImg.Value.ToLower().Replace("src=", "").Replace("\"", "").Replace("\'", "");

                        if (!src.StartsWith("http://") && !src.StartsWith("https://"))
                        {
                            //Insert new URL in img tag
                            src = "src=\"" + context.Request.Url.Scheme + "://" +
                                  context.Request.Url.Authority + src + "\"";
                            try
                            {
                                tempM = tempM.Remove(mImg.Index, mImg.Length);
                                tempM = tempM.Insert(mImg.Index, src);

                                //insert new url img tag in whole html code
                                tempInput = tempInput.Remove(m.Index, m.Length);
                                tempInput = tempInput.Insert(m.Index, tempM);
                            }
                            catch (Exception exception)
                            {
                                throw exception;
                            }
                        }
                    }
                }
            return tempInput;
        }

        public static string RemoveCssLink(this string html)
        {
            if (html == null)
                return string.Empty;
            var tempInput = html;
            foreach (Match m in Regex.Matches(tempInput, CssPattern))
                if (m.Success)
                {
                    var startIndex = m.Index;
                    tempInput = tempInput.Remove(startIndex, m.Length);
                }
            return tempInput;
        }

        public static IEnumerable<string> GetCssLinks(this string input)
        {
            var result = new List<string>();
            if (input == null)
                return result;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            var pattern1 = "href=[\'|\"](.+?)[\'|\"]";
            foreach (Match m in Regex.Matches(input, CssPattern))
                if (m.Success)
                {
                    var reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    var mImg = reImg.Match(m.Value);
                    var endString = mImg.Value;
                    endString = endString.Replace(" ", "");
                    var startIndex = endString.IndexOf("=", StringComparison.Ordinal);
                    endString = endString.Substring(startIndex + 1).Replace("\"", "");
                    result.Add(endString);
                }
            return FormatCssLinks(result);
        }

        private static IEnumerable<string> FormatCssLinks(List<string> cssLinks)
        {
            var localPath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var result = new List<string>();
            if (cssLinks == null) return result;
            foreach (var cssLink in cssLinks)
            {
                var src = cssLink.ToLower().Replace("href=", "").Replace("\"", "").Replace("\'", "");
                if (src.StartsWith("http://") || src.StartsWith("https://"))
                {
                    result.Add(src);
                }
                else
                {
                    result.Add(localPath + src);
                }
            }
            return result;
        }
    }
}