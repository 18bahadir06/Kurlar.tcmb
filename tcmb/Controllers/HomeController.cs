using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;                //model dosyasını dahil ettim
using Microsoft.Ajax.Utilities;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Web.Services.Protocols;
using System.Web.WebPages;
using System.Data.Entity.Migrations;
using System.Web.Helpers;
using System.Data.Entity;
using System.Reflection.Emit;
using System.Security.Policy;



namespace tcmb.Controllers
{
	public class HomeController : Controller
	{
		//dbkurEntities db = new dbkurEntities();

		string tarih;
		public ActionResult Index()
		{
			return View();
		}



		[HttpPost]
		public ActionResult Index()
		{
			if (Request["tarih"]!="")
			{
				var zmn = Request["tarih"];
				TempData["veri"] = zmn;
				return RedirectToAction("data", "Home", zmn);
			}
			else {
				return View();
			}
		}



		public ActionResult data(Kurlar model, Tarih model2)
		{
			//controls
			bool anacontrol = true,
				gzamancontrol = true,
				controltoday = false,
				haftasonucontrol = true;

			//url değişkeni
			string url;

			//zamanımız istenilen formatlara dönüştürüldü
			var zmn1 = TempData["veri"];
			string zmn=zmn1.ToString();
	
			//bu günün tarihi
			DateTime btarih = DateTime.Now;

			//tarih veri tabanında varmı yokmu
				
			var bilgilercontrol = db.Tarih.FirstOrDefault(x => x.Tarihler == zmn.ToString());
			//eğer veritabanında yoksa 

				
			if(bilgilercontrol==null)	
			{
				DateTime txttime = DateTime.Parse(zmn.ToString());
				int a = btarih.Year - txttime.Year;
				int b = btarih.Month - txttime.Month;
				int c = btarih.Day - txttime.Day;
				string gun = txttime.DayOfWeek.ToString();
				if (a < 0)    //SEÇİLEN TARİHİN GELECEK TEKİ ZAMAN KONTROLÜ	
				{
					gzamancontrol = false;
					anacontrol = false;
				}
				else if (a == 0 && b < 0)
				{
					gzamancontrol = false;
					anacontrol = false;
				}
				else if (a == 0 && b == 0 && c < 0)
				{
					gzamancontrol = false;
					anacontrol = false;
				}

				else if (gun == "Saturday" || gun == "Sunday")
				{
					haftasonucontrol= false;
					anacontrol = false;
				}
				//bu gün olup olmadığı control edildi
				if (a == 0 && b == 0 && c == 0)
				{
					gzamancontrol = true;
					anacontrol = true;
					controltoday = true;
				}


				//diziler oluşturuldu
				string[] Unit = new string[22];
				string[] Isim = new string[22];
				string[] ForexBuying = new string[22];
				string[] CurrencyName = new string[22];
				string[] ForexSelling = new string[22];
				string[] BanknoteBuying = new string[22];
				string[] BanknoteSelling = new string[22];


				//ay yıl gün değişkenleri
				string ay = "";
				string yil = "";
				string gunler = "";

				//yil ay gun degişlkenleri kontrolü
				if (txttime.Month < 10)
				{
					ay = "0" + txttime.Month.ToString();
				}

				else
				{
					ay = txttime.Month.ToString();
				}

				if (txttime.Day < 10)
				{
					gunler = "0" + txttime.Day.ToString();
				}

				else
				{
					gunler = txttime.Day.ToString();		
				}
					
				yil = txttime.Year.ToString();


				//url oluşturma
				if (anacontrol == true && bilgilercontrol==null 
					&& gzamancontrol==true && haftasonucontrol==true)
				{
					if (controltoday == true)
					{
						url = "http://www.tcmb.gov.tr/kurlar/today.xml";
					}
					else
					{
						//url:http://www.tcmb.gov.tr/kurlar/yyyyAA/GGAAYYYY.xml
						url = $"http://www.tcmb.gov.tr/kurlar/{yil}{ay}/{gunler}{ay}{yil}.xml";
					}
					//url ile verileri dizilere aktarma.
					try
					{
						var xml = new System.Xml.XmlDocument();
						xml.Load(url);

						for (int i = 0; i < 22; i++)
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/Unit";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							Unit[i] = usd;
						}

						for (int i = 0; i < 22; i++)
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/Isim";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							Isim[i] = usd;
						}


						//Currenciname için

						for (int i = 0; i < CurrencyName.Length; i++)
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/CurrencyName";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							CurrencyName[i] = usd;
						}

						//ForexBuying

						for (int i = 0; i < ForexBuying.Length; i++)	
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/ForexBuying";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							ForexBuying[i] = usd;
						}
						//ForexSelling

						for (int i = 0; i < ForexSelling.Length; i++)
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/ForexSelling";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							ForexSelling[i] = usd;
						}

						//BanknoteBuying

						for (int i = 0; i < BanknoteBuying.Length; i++)
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/BanknoteBuying";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							BanknoteBuying[i] = usd;
						}

						//BanknoteSelling

						for (int i = 0; i < BanknoteSelling.Length; i++)
						{
							string kod = "Tarih_Date/Currency [@CrossOrder= '" + i.ToString() + "']/BanknoteSelling";
							string usd = xml.SelectSingleNode(kod).InnerXml;
							BanknoteSelling[i] = usd;
						}
	
					}
						
					catch (Exception e)
					{
						ViewBag.mesaj= "bir hata oluştu yeniden denyiniz";
						return View(ViewBag.mesaj);
					}


					
					model2.Tarihler = zmn;
					db.Tarih.Add(model2);
					db.SaveChanges();
					List<Tarih> tarih2 = db.Tarih.ToList();
					int tarihid1 = tarih2.Count ;

					//tarihe ait tarihıd çekildi 


					for(int i = 0; i < 22; i++)
					{
						model.Birim = Unit[i].ToString();
						model.Dovizcins= Isim[i].ToString();
						model.Dovizalis= ForexBuying[i].ToString();
						model.Dovizsatis= ForexSelling[i].ToString();
						model.Efektifalis = BanknoteBuying[i].ToString();
						model.Efektifsatis = BanknoteSelling[i].ToString();
						model.TarihId= tarihid1;
						db.Kurlar.Add(model);
						db.SaveChanges();
					}


				}
				
			}
			
			if (anacontrol==true)
			{
				List<Tarih> tarih = db.Tarih.Where(x => x.Tarihler == zmn).ToList();

				string tarihid = "";

				//tarihe ait tarihıd çekildi 
				foreach (var i in tarih)
				{
					tarihid = i.TarihId.ToString();
				}
				//veri tabanından alınan kur bilgileri listelendi.

				List<Kurlar> kurlar = db.Kurlar.Where(z => z.TarihId.ToString() == tarihid).ToList();
				return View(kurlar);
			}
			else if (gzamancontrol == false)
			{
				ViewBag.mesaj = "seçtiğiniz tarihe ait kur bulunamamıştır lütfen geçmiş zaman seçiniz";
				return View();
			}
			else if (haftasonucontrol == false)
			{
				ViewBag.mesaj = "haftasonu kurlar hesaplanamaz";
				return View();
			}

			return View();
		}
	}
}