using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Leaf.Core.ExternalServices
{
    // TODO: rewrite all code

    #region Captcha Helpers
    public static class NewStringMethods
    {
        public static string ParseXml(this string str, string tag)
        {
            if (!string.IsNullOrEmpty(str) &&
                !string.IsNullOrEmpty(tag))
            {
                string left = "<" + tag + ">";
                string right = "</" + tag + ">";

                // Ищем начало позиции левой подстроки.
                int leftPosBegin = str.IndexOf(left, 0, StringComparison.Ordinal);
                if (leftPosBegin != -1)
                {
                    // Вычисляем конец позиции левой подстроки.
                    int leftPosEnd = leftPosBegin + left.Length;
                    // Ищем начало позиции правой строки.
                    int rightPos = str.IndexOf(right, leftPosEnd, StringComparison.Ordinal);

                    if (rightPos != -1)
                        return str.Substring(leftPosEnd, rightPos - leftPosEnd);
                }
            }
            throw new Exception("Tag not found");
        }
    }

    public struct RuCaptchaStats
    {
        public readonly int AvgTime;
        public readonly byte Load;
        public readonly float MinimalBid;
        public readonly int Waiting;

        public RuCaptchaStats(int avgTime, byte load, float minimalBid, int waiting)
        {
            AvgTime = avgTime;
            Load = load;
            MinimalBid = minimalBid;
            Waiting = waiting;
        }
    }
    #endregion

    public class Rucaptcha : IDisposable
    {
        #region Consts

        private readonly string _key; // = "a5ee8681d326547fec45f5e09329d159";

        private readonly CancellationToken _cancel;

        //private readonly string _key;
        const string _server = "rucaptcha.com";

        const int _tryCount = 40,
                  _tryCountReady = _tryCount * 2,
                  _waitMsecBeforeRequest = 3000;

        #endregion

        #region Variables
        private readonly WebClient _webClient = new WebClient();
        
                                                       
        string _lastCaptchaId;
        #endregion
       
        public Rucaptcha(string key, CancellationToken cancel)
        {
            _key = key;
            _cancel = cancel;            
        }

        #region Static Methods
        public static RuCaptchaStats GetStatistics()
        {
            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.DownloadString("http://" + _server + "/load.php");
            }

            int avgTime = int.Parse(response.ParseXml("averageRecognitionTime"));
            byte load = byte.Parse(response.ParseXml("load"));
            float minimalBid = float.Parse(response.ParseXml("minbid").Replace('.',','));            
            int waiting = int.Parse(response.ParseXml("waiting"));            

            return new RuCaptchaStats(avgTime, load, minimalBid, waiting);
        }
        #endregion

        #region Non-Static Methods

        public string GetAllStatistics()
        {
            var stats = GetStatistics();
            return string.Format("Баланс RuCaptcha: {0} руб.{5}Bid: {1}; Load: {2}%; AVG: {3}; Waiting: {4}",
                GetBalance(), stats.MinimalBid, stats.Load, stats.AvgTime, stats.Waiting, Environment.NewLine);
        }

        public string GetBalance()
        {
            string response;
            WebClient webClient = null;

            _cancel.ThrowIfCancellationRequested();

            try
            {
                webClient = new WebClient();
                response = webClient.DownloadString("http://" + _server + "/res.php?key=" + _key + "&action=getbalance");
            }
            catch
            {
                response = "Невозможно получить баланс Rucaptcha";
            }
            finally
            {
                webClient?.Dispose();
            }
            return response;
        }

        public string ReportLastCaptcha()
        {
            return _webClient.DownloadString("http://" + _server + "/res.php?key=" + _key + "&action=reportbad&id=" + _lastCaptchaId);
        }

        public string Recognize(Image image, int minLength = 6, int maxLength = 6, bool onlyLetters = false, bool phrase = false, bool russian = false)
        {
            // convert image to array of bytes
            byte[] imageData;
            using (var stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                imageData = stream.ToArray();
            }
            return Recognize(imageData, minLength, maxLength, onlyLetters, phrase, russian);
        }

        public string Recognize(byte[] imageData, int minLength = 6, int maxLength = 6, bool onlyLetters = false, bool phrase = false, bool russian = false)
        {       
            // sending image
            var postValues = new NameValueCollection
            {
                { "key", _key },
                //{ "regsense", "0" },
                { "method", "base64" },
                { "body", Convert.ToBase64String(imageData) },
                { "min_len", minLength.ToString()},
                { "max_len", maxLength.ToString()},
                { "language", russian ? "1" : "2"},
                { "numeric", !onlyLetters ? "0" : "2"},
                { "phrase", !phrase ? "0" : "1"}
            };

            string result = "unknown";
            bool fatalError = true;

            for (int i = 0; i < _tryCount; i++)
            {
                result = Encoding.UTF8.GetString(_webClient.UploadValues("http://" + _server + "/in.php", postValues));

                if (!result.Contains("ERROR_NO_SLOT_AVAILABLE"))
                {
                    fatalError = !result.Contains("OK|");
                    break;
                }

                _cancel.ThrowIfCancellationRequested();
                Thread.Sleep(_waitMsecBeforeRequest);
            }
            if (fatalError)
                throw new Exception("Ошибка загрузки RuCaptcha: " + result);

            _lastCaptchaId = result.Replace("OK|", "").Trim();

            fatalError = true;
            Thread.Sleep(_waitMsecBeforeRequest * 2);

            for (int i = 0; i < _tryCountReady; i++)
            {
                result = _webClient.DownloadString("http://" + _server + "/res.php?key=" + _key + "&action=get&id=" + _lastCaptchaId);

                if (!result.Contains("CAPCHA_NOT_READY"))
                {
                    fatalError = !result.Contains("OK|");
                    break;
                }
                _cancel.ThrowIfCancellationRequested();
                Thread.Sleep(_waitMsecBeforeRequest);
            }

            _cancel.ThrowIfCancellationRequested();

            if (fatalError)
                throw new Exception("Ошибка распознавания RuCaptcha: " + result ?? "unknown");

            return result.Replace("OK|", "").Trim();
        }

        public string Recognize(string imageBase64)
        {
            // convert image to array of bytes
            /*
            byte[] imageData;
            using (var stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                imageData = stream.ToArray();
            }*/
            
            // sending image
            var postValues = new NameValueCollection
            {
                { "key", _key },
                { "method", "base64" },
                { "body", imageBase64 },
                { "language", "1"},
                { "min_len", "5"},
                { "max_len", "6"}
            };

            string result = "unknown";
            bool fatalError = true;

            for (int i = 0; i < _tryCount; i++)
            {
                result = Encoding.UTF8.GetString(_webClient.UploadValues("http://" + _server + "/in.php", postValues));

                if (!result.Contains("ERROR_NO_SLOT_AVAILABLE"))
                {
                    fatalError = !result.Contains("OK|");
                    break;
                }

                _cancel.WaitHandle.WaitOne(_waitMsecBeforeRequest);
            }
            if (fatalError)
                throw new Exception("Ошибка загрузки RuCaptcha: " + result);

            _lastCaptchaId = result.Replace("OK|", "").Trim();

            fatalError = true;

            _cancel.WaitHandle.WaitOne(_waitMsecBeforeRequest);

            for (int i = 0; i < _tryCountReady; i++)
            {
                result = _webClient.DownloadString("http://" + _server + "/res.php?key=" + _key + "&action=get&id=" + _lastCaptchaId);

                if (!result.Contains("CAPCHA_NOT_READY"))
                {
                    fatalError = !result.Contains("OK|");
                    break;
                }

                _cancel.WaitHandle.WaitOne(_waitMsecBeforeRequest);
            }

            _cancel.ThrowIfCancellationRequested();
            if (fatalError)
                throw new CaptchaException("Ошибка распознавания RuCaptcha: " + result ?? "unknown");
            
            return result.Replace("OK|", "").Trim().ToUpper();
        }

        public string RecognizeRecaptcha(string key, string url)
        {
            // http://rucaptcha.com/in.php?key=YOUR_CAPTCHA_KEY&method=userrecaptcha&googlekey=%googlekey%

            var postValues = new NameValueCollection
            {
                { "key", _key },
                { "method", "userrecaptcha" },
                { "googlekey", key },               
                //{ "proxy", "149.202.249.206:3128" },
                //{ "proxytype", "HTTP"    },
                { "pageurl", url },
            };

            string result = "unknown";
            bool fatalError = true;

            for (int i = 0; i < _tryCount; i++)
            {
                result = Encoding.UTF8.GetString(_webClient.UploadValues("http://" + _server + "/in.php", postValues));

                if (!result.Contains("ERROR_NO_SLOT_AVAILABLE"))
                {
                    fatalError = !result.Contains("OK|");
                    break;
                }

                _cancel.WaitHandle.WaitOne(_waitMsecBeforeRequest);
            }

            if (fatalError)
                throw new Exception("Ошибка загрузки RuCaptcha: " + result);

            _lastCaptchaId = result.Replace("OK|", "").Trim();

            fatalError = true;
            _cancel.WaitHandle.WaitOne(_waitMsecBeforeRequest * 2);

            for (int i = 0; i < _tryCountReady * 2; i++)
            {
                result = _webClient.DownloadString("http://" + _server + "/res.php?key=" + _key + "&action=get&id=" + _lastCaptchaId);

                if (!result.Contains("CAPCHA_NOT_READY"))
                {
                    fatalError = !result.Contains("OK|");
                    break;
                }

                _cancel.WaitHandle.WaitOne(_waitMsecBeforeRequest);
            }

            _cancel.ThrowIfCancellationRequested();

            if (fatalError)
                throw new CaptchaException("Ошибка распознавания RuCaptcha: " + result);

            string answer = result.Replace("OK|", "");
            if (string.IsNullOrEmpty(answer))
                throw new CaptchaException("Ошибка распознавания RuCaptcha: возвращен пустой ответ");

            return answer;
        }
        #endregion

        #region Destructor
        bool _disposed;
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _webClient?.Dispose();

            // Free any unmanaged objects here.
            //
            _disposed = true;
        }
        #endregion
    }
}
