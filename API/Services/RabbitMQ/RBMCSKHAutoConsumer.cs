

//using System.Diagnostics;
//using System.Text;
//using System.Text.RegularExpressions;
//using API.Constant;
//using API.Data;
//using API.Models.CSKHAuto;
//using API.Models.CSKHAuto.ReceiptBots;
//using API.Utility;
//using Microsoft.AspNetCore.Connections;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Newtonsoft.Json;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using Serilog;
//using IModel = RabbitMQ.Client.IModel;
//using SD = API.Constant.SD;
//using StringToolkit = API.Constant.StringToolkit;

//namespace API.Services.RabbitMQ
//{
//    public class RBMCSKHAutoConsumer : BackgroundService
//    {
//        private IConnection _connection;
//        private IModel _channel;
//        private readonly IHostEnvironment _env;
//        private AppDbContext _db;
//        private IServiceProvider _serviceProvider;

//        public RBMCSKHAutoConsumer(IServiceProvider ServiceProvider, IHostEnvironment env)
//        {
//            _env = env;

//            var factory = new ConnectionFactory
//            {
//                HostName = "localhost",
//                Password = "guest",
//                UserName = "guest",
//            };
//            _connection = factory.CreateConnection();
//            _channel = _connection.CreateModel();
//            _channel.ExchangeDeclare(SD.DepositReceipt_Upload_ExchangeName, ExchangeType.Direct);
//            _channel.QueueDeclare("CSKH_AUTOMATICALLY", false, false, false, null);
//            _channel.QueueBind("CSKH_AUTOMATICALLY", SD.DepositReceipt_Upload_ExchangeName, "RequestCommand");
//            _serviceProvider = ServiceProvider;
//        }

//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();
//            var _DepositMessage = new EventingBasicConsumer(_channel);
//            _DepositMessage.Received += (ch, ea) =>
//            {
//                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
//                TicketRequest _TicketDepositReceipt = JsonConvert.DeserializeObject<TicketRequest>(content);
//                HandleRequestMessage(_TicketDepositReceipt).GetAwaiter().GetResult();
//                _channel.BasicAck(ea.DeliveryTag, false);
//            };
//            _channel.BasicConsume("CSKH_AUTOMATICALLY", false, _DepositMessage);
//            return Task.CompletedTask;
//        }

//        private async Task HandleRequestMessage(TicketRequest iTickeDepositReceipt)
//        {
//            try
//            {
//                _db = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

//                List<PatternRegex> RegexPattern = await _db.PatternRegexs.ToListAsync();

//                ReceiptBot teleReceiptBot = await _db.ReceiptBots.Where(x => x.Id == 1).FirstOrDefaultAsync();

//                //Convert Image String to Base64
//                byte[] Base64Image = ImageToolkit.Base64StringToBytes(iTickeDepositReceipt.UploadImage);

//                //Get Extension
//                string extension = ImageToolkit.GetImageExtensionFromBase64(iTickeDepositReceipt.UploadImage);

//                //Set name 
//                string Filename = $"{Guid.NewGuid()}{extension}";

//                //Set local file path to download
//                string fileLocalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DepositReceiptIMG", Filename);

//                //Set out file
//                string outLocalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DepositReceiptIMG/out.txt");

//                while (System.IO.File.Exists(fileLocalPath))
//                {
//                    try
//                    {
//                        GC.Collect();
//                        GC.WaitForPendingFinalizers();
//                        System.IO.File.Delete(fileLocalPath);
//                    }
//                    catch (Exception ex1)
//                    {
//                        Log.Information(fileLocalPath + "\n" + ex1.Message);
//                    }
//                    Thread.Sleep(200);
//                }

//                await System.IO.File.WriteAllBytesAsync(fileLocalPath, Base64Image);
//                Log.Information($"DOWNLOAD IMAGE RECEIPT FILE");

//                //tesseract.exe -l vie 166725.jpg out
//                string fileName = Path.GetFileName(fileLocalPath);
//                string TesseractApp = _env.ContentRootPath + @"\wwwroot\DepositReceiptIMG\tesseract.exe";
//                string DirectoryApp = Path.GetDirectoryName(TesseractApp);
//                Process process = new Process();
//                process.StartInfo.RedirectStandardOutput = true;
//                process.StartInfo.UseShellExecute = false;
//                process.StartInfo.CreateNoWindow = true;
//                process.StartInfo.FileName = TesseractApp;
//                process.StartInfo.Arguments = "-l vie " + fileName + " out";
//                process.StartInfo.WorkingDirectory = DirectoryApp;
//                process.Start();
//                process.WaitForExit();

//                if (System.IO.File.Exists(outLocalPath))
//                {
//                    TicketRequest _tempTickeDepositReceipt = new TicketRequest();
//                    bool fAllowToSend = true;
//                    string OCRData = "🛎 Tài khoản: " + iTickeDepositReceipt.Account + "\n";

//                    if (!string.IsNullOrEmpty(iTickeDepositReceipt.CustomerNote))
//                    {
//                        OCRData += "🛎 Ghi chú: " + iTickeDepositReceipt.CustomerNote + "\n";
//                    }
//                    string OCRDataContent = System.IO.File.ReadAllText(outLocalPath);
//                    OCRDataContent = OCRDataContent.Replace("\n\n", "\n");

//                    _tempTickeDepositReceipt.CustomerNote = OCRDataContent;

//                    if (RegexPattern.Count > 0)
//                    {
//                        for (int j = 0; j < RegexPattern.Count; j++)
//                        {
//                            StringBuilder patternBuilder = new StringBuilder();
//                            patternBuilder.Append(RegexPattern[j].RegexPattern);
//                            Regex tempRegex = new Regex(patternBuilder.ToString());
//                            MatchCollection matchRegex = tempRegex.Matches(OCRDataContent.ToUpper());
//                            if (matchRegex.Count > 0)
//                            {
//                                OCRData += $"🛎 {RegexPattern[j].Note}: ";
//                                for (int k = 0; k < matchRegex.Count; k++)
//                                {
//                                    OCRData += " [" + matchRegex[k].Value + "]";
//                                    //if (!string.IsNullOrEmpty(_tempTickeDepositReceipt.Keyword))
//                                    //{
//                                    //    if (_tempDepositReceiptMessage.Keyword.IndexOf(matchRegex[k].Value) == -1)
//                                    //    {
//                                    //        _tempDepositReceiptMessage.Keyword += ("|" + matchRegex[k].Value);
//                                    //    }
//                                    //}
//                                    //else
//                                    //{
//                                    //    _tempDepositReceiptMessage.Keyword += ("|" + matchRegex[k].Value);
//                                    //}
//                                }
//                                OCRData += "\n";
//                            }
//                            else
//                            {
//                                //There is no matching regex
//                                if (RegexPattern[j].Note == "KEY_WORD")
//                                {
//                                    Log.Information("NO KEY WORD FOUND");
//                                    fAllowToSend = false;

//                                    string imgPath = $"wwwroot/DepositReceiptIMG/{Filename}";
//                                    File.Delete(imgPath);

//                                    break;
//                                }
//                            }
//                        }
//                    }

//                    if (fAllowToSend == true)
//                    {
//                        //List<DepositReceiptMessage> tempDepositReceipMessageSimilarList = await _db.DepositReceiptMessages.Where(x => EF.Functions.Like(x.Keyword, _tempTickeDepositReceipt.Keyword.ToString())).ToListAsync();
//                        List<DepositReceiptMessage> _tempTickeDepositReceiptSimilarList = await _db.DepositReceiptMessages.ToListAsync();
//                        for (int i = 0; i < _tempTickeDepositReceiptSimilarList.Count; i++)
//                        {
//                            double compareResult = StringToolkit.GetJaccardSimilarity(_tempTickeDepositReceiptSimilarList[i].Content, _tempTickeDepositReceipt.CustomerNote);
//                            if (compareResult * 100 > 95 && iTickeDepositReceipt.System == _tempTickeDepositReceiptSimilarList[i].System)
//                            {
//                                Log.Information("FOUND DUPLICATE KEYWORD");
//                                fAllowToSend = false;

//                                string imgPath = $"wwwroot/DepositReceiptIMG/{Filename}";

//                                File.Delete(imgPath);

//                                break;
//                            }
//                        }
//                    }

//                    if (fAllowToSend == true)
//                    {
//                        if (iTickeDepositReceipt.System == "F8BET_DEPOSIT_RECEIPT")
//                        {
//                            OCRData += "🛎 @kawaitcn @duppe123 \n";
//                        }

//                        if (teleReceiptBot != null)
//                        {
//                            await CheckReceiptSendMessageAsync(teleReceiptBot, Filename, OCRData + "\n-------------\n" + OCRDataContent);
//                        }

//                        _tempTickeDepositReceipt.Account = iTickeDepositReceipt.Account;
//                        _tempTickeDepositReceipt.UploadImage = Filename;
//                        _tempTickeDepositReceipt.TicketCode = iTickeDepositReceipt.TicketCode;
//                        _tempTickeDepositReceipt.RequestDate = DateTime.Now;

//                        _tempTickeDepositReceipt.System = iTickeDepositReceipt.System;
//                        _tempTickeDepositReceipt.CustomerNote = iTickeDepositReceipt.CustomerNote;
//                        _tempTickeDepositReceipt.Phone = iTickeDepositReceipt.Phone;

//                        await _db.TicketRequests.AddAsync(_tempTickeDepositReceipt);
//                        await _db.SaveChangesAsync();
//                    }

//                    while (System.IO.File.Exists(outLocalPath))
//                    {
//                        try
//                        {
//                            GC.Collect();
//                            GC.WaitForPendingFinalizers();
//                            System.IO.File.Delete(outLocalPath);
//                        }
//                        catch (Exception ex1)
//                        {
//                            Log.Information(outLocalPath + "\n" + ex1.Message);
//                        }
//                        Thread.Sleep(200);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("READ DATA FAILED");
//                    await CheckReceiptSendMessageAsync(teleReceiptBot, Filename, "READ DATA FAILED");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.InnerException.Message);
//            }
//        }


///*        private async Task HandleRequestMessage(TicketRequest iTickeDepositReceipt)
//        {
//            try
//            {
//                _db = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

//                List<PatternRegex> RegexPattern = await _db.PatternRegexs.ToListAsync();

//                ReceiptBot teleReceiptBot = await _db.ReceiptBots.Where(x => x.Id == 1).FirstOrDefaultAsync();

//                //Convert Image String to Base64
//                byte[] Base64Image = ImageToolkit.Base64StringToBytes(iTickeDepositReceipt.UploadImage);

//                //Get Extension
//                string extension = ImageToolkit.GetImageExtensionFromBase64(iTickeDepositReceipt.UploadImage);

//                //Set name 
//                string Filename = $"{Guid.NewGuid()}{extension}";

//                //Set local file path to download
//                string fileLocalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DepositReceiptIMG", Filename);

//                //Set out file
//                string outLocalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/DepositReceiptIMG/out.txt");

//                while (System.IO.File.Exists(fileLocalPath))
//                {
//                    try
//                    {
//                        GC.Collect();
//                        GC.WaitForPendingFinalizers();
//                        System.IO.File.Delete(fileLocalPath);
//                    }
//                    catch (Exception ex1)
//                    {
//                        Log.Information(fileLocalPath + "\n" + ex1.Message);
//                    }
//                    Thread.Sleep(200);
//                }

//                await System.IO.File.WriteAllBytesAsync(fileLocalPath, Base64Image);
//                Log.Information($"DOWNLOAD IMAGE RECEIPT FILE");

//                //tesseract.exe -l vie 166725.jpg out
//                string fileName = Path.GetFileName(fileLocalPath);
//                string TesseractApp = _env.ContentRootPath + @"\wwwroot\DepositReceiptIMG\tesseract.exe";
//                string DirectoryApp = Path.GetDirectoryName(TesseractApp);
//                Process process = new Process();
//                process.StartInfo.RedirectStandardOutput = true;
//                process.StartInfo.UseShellExecute = false;
//                process.StartInfo.CreateNoWindow = true;
//                process.StartInfo.FileName = TesseractApp;
//                process.StartInfo.Arguments = "-l vie " + fileName + " out";
//                process.StartInfo.WorkingDirectory = DirectoryApp;
//                process.Start();
//                process.WaitForExit();

//                if (System.IO.File.Exists(outLocalPath))
//                {
//                    TicketRequest _tempTickeDepositReceipt = new TicketRequest();
//                    bool fAllowToSend = true;
//                    string OCRData = "🛎 Tài khoản: " + iTickeDepositReceipt.Account + "\n";

//                    if (!string.IsNullOrEmpty(iTickeDepositReceipt.CustomerNote))
//                    {
//                        OCRData += "🛎 Ghi chú: " + iTickeDepositReceipt.CustomerNote + "\n";
//                    }
//                    string OCRDataContent = System.IO.File.ReadAllText(outLocalPath);
//                    OCRDataContent = OCRDataContent.Replace("\n\n", "\n");

//                    _tempTickeDepositReceipt.CustomerNote = OCRDataContent;

//                    if (RegexPattern.Count > 0)
//                    {
//                        for (int j = 0; j < RegexPattern.Count; j++)
//                        {
//                            StringBuilder patternBuilder = new StringBuilder();
//                            patternBuilder.Append(RegexPattern[j].RegexPattern);
//                            Regex tempRegex = new Regex(patternBuilder.ToString());
//                            MatchCollection matchRegex = tempRegex.Matches(OCRDataContent.ToUpper());
//                            if (matchRegex.Count > 0)
//                            {
//                                OCRData += $"🛎 {RegexPattern[j].Note}: ";
//                                for (int k = 0; k < matchRegex.Count; k++)
//                                {
//                                    OCRData += " [" + matchRegex[k].Value + "]";

//                                    // Setting Re
//                                }
//                                OCRData += "\n";
//                            }
//                            else
//                            {
//                                //There is no matching regex
//                                if (RegexPattern[j].Note == "KEY_WORD")
//                                {
//                                    Log.Information("NO KEY WORD FOUND");
//                                    fAllowToSend = false;

//                                    string imgPath = $"wwwroot/DepositReceiptIMG/{Filename}";
//                                    File.Delete(imgPath);

//                                    break;
//                                }
//                            }
//                        }
//                    }

//                    if (fAllowToSend == true)
//                    {
//                        //List<DepositReceiptMessage> tempDepositReceipMessageSimilarList = await _db.DepositReceiptMessages.Where(x => EF.Functions.Like(x.Keyword, _tempTickeDepositReceipt.Keyword.ToString())).ToListAsync();
//                        List<DepositReceiptMessage> _tempTickeDepositReceiptSimilarList = await _db.DepositReceiptMessages.ToListAsync();
//                        for (int i = 0; i < _tempTickeDepositReceiptSimilarList.Count; i++)
//                        {
//                            double compareResult = StringToolkit.GetJaccardSimilarity(_tempTickeDepositReceiptSimilarList[i].Content, _tempTickeDepositReceipt.CustomerNote);
//                            if (compareResult * 100 > 95 && iTickeDepositReceipt.System == _tempTickeDepositReceiptSimilarList[i].System)
//                            {
//                                Log.Information("FOUND DUPLICATE KEYWORD");
//                                fAllowToSend = false;

//                                string imgPath = $"wwwroot/DepositReceiptIMG/{Filename}";

//                                File.Delete(imgPath);

//                                break;
//                            }
//                        }
//                    }

//                    if (fAllowToSend == true)
//                    {
//                        if (iTickeDepositReceipt.System == "F8BET_DEPOSIT_RECEIPT")
//                        {
//                            OCRData += "🛎 @kawaitcn @duppe123 \n";
//                        }

//                        if (teleReceiptBot != null)
//                        {
//                            await CheckReceiptSendMessageAsync(teleReceiptBot, Filename, OCRData + "\n-------------\n" + OCRDataContent);
//                        }

//                        _tempTickeDepositReceipt.Account = iTickeDepositReceipt.Account;
//                        _tempTickeDepositReceipt.UploadImage = Filename;
//                        _tempTickeDepositReceipt.TicketCode = iTickeDepositReceipt.TicketCode;
//                        _tempTickeDepositReceipt.RequestDate = DateTime.Now;

//                        _tempTickeDepositReceipt.System = iTickeDepositReceipt.System;
//                        _tempTickeDepositReceipt.CustomerNote = iTickeDepositReceipt.CustomerNote;
//                        _tempTickeDepositReceipt.Phone = iTickeDepositReceipt.Phone;

//                        await _db.TicketRequests.AddAsync(_tempTickeDepositReceipt);
//                        await _db.SaveChangesAsync();
//                    }

//                    while (System.IO.File.Exists(outLocalPath))
//                    {
//                        try
//                        {
//                            GC.Collect();
//                            GC.WaitForPendingFinalizers();
//                            System.IO.File.Delete(outLocalPath);
//                        }
//                        catch (Exception ex1)
//                        {
//                            Log.Information(outLocalPath + "\n" + ex1.Message);
//                        }
//                        Thread.Sleep(200);
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("READ DATA FAILED");
//                    await CheckReceiptSendMessageAsync(teleReceiptBot, Filename, "READ DATA FAILED");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.InnerException.Message);
//            }
//        }
//*/
//        private async Task<string> CheckReceiptSendMessageAsync(ReceiptBot ReceiptImageBot, string ImagePath, string ContentText)
//        {
//            try
//            {
//                HttpClient client = new HttpClient();
//                HttpRequestMessage message = new HttpRequestMessage();
//                message.Method = HttpMethod.Post;
//                message.Headers.Add("Accept", "application/json");
//                message.RequestUri = new Uri("https://api.telegram.org/bot" + ReceiptImageBot.BotToken + "/sendPhoto");
//                message.Content = JsonContent.Create(new Dictionary<string, object>
//                {
//                    ["photo"] = $"https://cskh-api.ttkmvip.org/DepositReceiptIMG/{ImagePath}",
//                    ["chat_id"] = ReceiptImageBot.ChatID.ToString(),
//                    ["caption"] = ContentText
//                });
//                var response = await client.SendAsync(message);
//                Log.Information(response.StatusCode.ToString());
//            }
//            catch (Exception ex)
//            {
//                Log.Information(ex.ToString());
//            }

//            return "";
//        }

//    }
//}
