using System;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SaaPalvelu
{
    public class Interface
    {
        /// <summary>
        /// Ohjelma kysyy käyttäjältä kaupungin nimen, josta käyttä haluaa säätiedot ja tulostaa ne käyttäjälle nähtäväksi.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Tervetuloa Päivän sää-palveluun! ");
            bool loppu = false;
            while (loppu == false)
            {
                Console.Write("Syötä kaupunki josta haluat sääteidot: ");
                string city = Console.ReadLine();
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                string data = APIWeather(city);
                if (data == "404") continue;
                InputData(data);
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Ask();
                while (true)
                {
                    int[] answers = ForecastGuess();
                    int[] dataF = APIForecast(city);
                    Console.WriteLine("----------------------------------------------------------------------------------------------------");
                    int lkm = Compare(answers, dataF);
                    Console.WriteLine("----------------------------------------------------------------------------------------------------");
                    if (lkm > 0) Console.WriteLine("Hienoa! Sait " + lkm + " oikein!");
                    else Console.WriteLine("Voi kurja, et saanut yhtään oikein.");
                    Console.WriteLine("Haluatko pelata uudestaan?");
                    string again = Console.ReadLine();
                    if (again == "e" || again == "E")
                    {
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");
                        ShowResults(dataF); 
                        break;
                    }
                    Console.WriteLine("----------------------------------------------------------------------------------------------------");
                }
                loppu = true;
            }

        }


        /// <example>
        /// <pre name="test">
        ///  string syote = "Helsinki";
        ///  APIWeather(syote).Contains("200") === true;
        ///  syote = "Kalkuta";
        ///  APIWeather(syote).Contains("200") === true;
        ///  syote = "Helk";
        ///  APIWeather(syote).Contains("404") === true;
        ///  syote = "Jyvskylä"; 
        /// APIWeather(syote).Contains("200") === false;
        /// </pre>
        /// </example>
        /// <summary>
        /// Tässä haetaan tiedot Openweathermap-palvelun 
        /// API:sta ja palautetaan ne merkkijonona parserointifunktion käyttöön.
        /// </summary>
        /// <param name="syote"></param>
        public static string APIWeather(string syote)
        {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={syote}&units=metric&lang=fi&APPID=36d53058c9009ea0daf89ba0d9a1e873";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string responseData = "";
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                System.IO.Stream responseS = response.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(responseS);
                responseData = sr.ReadToEnd();

            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message + " --- Virhe tietojen hakemisessa.");
                responseData = "404";
                
            }
            return responseData;
        }


        /// <summary>
        /// Tässä tehdään samoin, kuin APIWeather-funktiossa, 
        /// mutta palautetaan huomisen päivän sääennuste takaisin taulukossa.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static int [] APIForecast(string city)
        {
            int[] dataF = new int[3];
            string url = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&units=metric&lang=fi&APPID=36d53058c9009ea0daf89ba0d9a1e873";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            System.IO.Stream responseS = response.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(responseS);
            string responseData = sr.ReadToEnd();

            JObject dataObject = JObject.Parse(responseData);

            JArray list = (JArray)dataObject["list"];
            JObject main = (JObject)list[9]["main"];
            int temp = (int)main["temp"];
            dataF[0] = temp;
            int humidity = (int)main["humidity"];
            dataF[1] = humidity;

            JObject wind = (JObject)list[2]["wind"];
            int speed = (int)wind["speed"];
            dataF[2] =  speed;



            return dataF;
        }


        /// <summary>
        /// Funktio ottaa parametrinä API:sta tulleen datan, erittelee
        /// halutut säätiedot ja tulostaa ne konsoliin.
        /// </summary>
        /// <param name="responseData"></param>
        public static void InputData(string responseData)
        {
            JObject data = JObject.Parse(responseData);

            JObject main = (JObject)data["main"];
            int temp = (int)main["temp"];
            int feels = (int)main["feels_like"];
            int humidity = (int)main["humidity"];

            JObject wind = (JObject)data["wind"];
            int speed = (int)wind["speed"];

            JArray weather = (JArray)data["weather"];
            string desc = (string)weather[0]["description"];

            Console.WriteLine("Tällä hetkellä sää näyttäisi olevan " + desc);
            Console.WriteLine("Lämpötila tällä hetkellä on " + temp + "°");
            Console.WriteLine("Tuntuu kuin olisi " + feels + "°");
            Console.WriteLine("Ilman kosteus on " + humidity + "%");
            Console.WriteLine("Tuulen nopeus on " + speed + "m/s");


        }


        /// <summary>
        /// Kysytään käyttäjältä, haluaako hän arvata ennusteen
        /// ja talletetaan arvaukset listaan. Tätä listaa voidaan käyttää 
        /// vertailufunktiossa.
        /// </summary>
        /// <returns></returns>
        public static int[] ForecastGuess()
        {
            
            int[] answers = new int[3];

            bool valmis = false;

            while (valmis == false)
            {
                try
                {
                    Console.Write("Anna lämpötila (C): ");
                    string temp = Console.ReadLine();
                    int itemp = Convert.ToInt32(temp);
                    answers[0] = itemp;
                    Console.Write("Anna ilman kosteus : ");
                    string wind = Console.ReadLine();
                    int iwind = Convert.ToInt32(wind);
                    answers[1] = iwind;
                    Console.Write("Anna tuulen nopeus: ");
                    string humidity = Console.ReadLine();
                    int ihumidity = Convert.ToInt32(humidity);
                    answers[2] = ihumidity;
                    valmis = true;

                }
                catch
                {
                    Console.WriteLine("Syötä kokonaisluku!");
                }
            }

            return answers;
        }


        /// <summary>
        /// Otetaan parametrina vastaustaulukko ja ennustedata.
        /// Suoritetaan vertailu, tulostetaan tulos
        /// ja lasketaan kuinka monta niistä osui oikeaan.
        /// Lopuksi palautetaan lukumäärä, jotta voidaan 
        /// tulostaa viesti lopullisesta tuloksesta.
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <example>
        /// <pre name="test">
        /// int[] answers = {3, 20, 14};
        /// int[] dataF = {3, 45, 4};
        /// Compare(answers, dataF) === 1;
        /// int[] answerss = {12, 41, 5};
        /// int[] data = {12, 41, 4};
        /// Compare(answerss, data) === 2;
        /// </pre>
        /// </example>
        public static int Compare(int[] answers, int[] data )
        {
            int lkm = 0;
            for (int i = 0; i < answers.Length; i++)
            {
                if (answers[i] != data[i])
                {
                    Console.WriteLine("Vastasit " + (i+1) + ". kysymykseen " + answers[i] + ", joka oli väärin." );
                }
                else
                {
                    Console.WriteLine("Vastasit " + (i+1) + ". kysymykseen " + answers[i] + ", se oli oikein!");
                    lkm++;
                }
            }

            return lkm;
        }

        /// <summary>
        /// Aliohjelma kysyy käyttäjältä, haluaako hän
        /// arvata huomisen sään. Jos vastaus on kielteinen, niin 
        /// ohjelma lopetetaan.
        /// </summary>
        public static void Ask()
        {
            string input = "";
            Console.Write("Haluatko arvata huomisen sään? (K/E)");

            input = Console.ReadLine();

            if (input == "E" || input == "e")
            {
                Console.WriteLine("Heissulivei!");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Aliohjelma ottaa parametrina ennusteen vastaukset
        /// ja kertoo käyttäjälle oikeat vastaukset.
        /// </summary>
        /// <param name="data"></param>
        public static void ShowResults(int[] data)
        {
            Console.WriteLine("Ensimmäisen kysymyksen vastaus: " + data[0]);
            Console.WriteLine("Toisen kysymyksen vastaus: " + data[1]);
            Console.WriteLine("Kolmannen kysymyksen vastaus: " + data[2]);
        }
    }
}
