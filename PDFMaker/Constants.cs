using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace hashmakersol.pdfmaker
{
    public static class Constants
    {
        /// <summary>
        ///     Convert relative link to absolute link
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string FormatImageLinks(string input)
        {
            if (input == null)
                return string.Empty;
            var tempInput = input;
            const string pattern = @"<img(.|\n)+?>";
            var context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (
                Match m in
                Regex.Matches(input, pattern,
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
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            return tempInput;
        }

        public static string FormatCSSLinks(string input)
        {
            if (input == null)
                return string.Empty;
            var tempInput = input;
            const string pattern = @"<link(.|\n)+?/>";
            var context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (
                Match m in
                Regex.Matches(input, pattern,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft))
                if (m.Success)
                {
                    var tempM = m.Value;
                    var pattern1 = "href=[\'|\"](.+?)[\'|\"]";
                    var reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    var mImg = reImg.Match(m.Value);

                    if (mImg.Success)
                    {
                        var src = mImg.Value.ToLower().Replace("href=", "").Replace("\"", "").Replace("\'", "");

                        if (!src.StartsWith("http://") && !src.StartsWith("https://"))
                        {
                            //Insert new URL in img tag
                            src = "href=\"" + context.Request.Url.Scheme + "://" +
                                  context.Request.Url.Authority + src + "\"";
                            try
                            {
                                tempM = tempM.Remove(mImg.Index, mImg.Length);
                                tempM = tempM.Insert(mImg.Index, src);

                                //insert new url img tag in whole html code
                                tempInput = tempInput.Remove(m.Index, m.Length);
                                tempInput = tempInput.Insert(m.Index, tempM);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            return tempInput;
        }

        public static List<string> GetCSSLinks(string input)
        {
            var result = new List<string>();
            if (input == null)
                return result;

            input = FormatCSSLinks(input);
            var tempInput = input;
            const string pattern = @"<link(.|\n)+?\/>";
            var context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (Match m in Regex.Matches(input, pattern))
                if (m.Success)
                {
                    var pattern1 = "href=[\'|\"](.+?)[\'|\"]";
                    var reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    var mImg = reImg.Match(m.Value);
                    var endString = mImg.Value;

                    endString = endString.Replace(" ", "");
                    var startIndex = endString.IndexOf("=");
                    endString = endString.Substring(startIndex + 1).Replace("\"", "");
                    result.Add(endString);
                }
            return result;
        }
    }
}