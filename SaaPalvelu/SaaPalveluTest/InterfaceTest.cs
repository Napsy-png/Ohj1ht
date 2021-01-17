using System;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using static SaaPalvelu.Interface;
using SaaPalvelu;

namespace TestSaaPalvelu
{
	[TestFixture]
	public  class TestInterface
	{
		[Test]
		public  void TestAPIWeather57()
		{
			string syote = "Helsinki";
			Assert.AreEqual( true, APIWeather(syote).Contains("200") , "in method APIWeather, line 59");
			syote = "Kalkuta";
			Assert.AreEqual( true, APIWeather(syote).Contains("200") , "in method APIWeather, line 61");
			syote = "Helk";
			Assert.AreEqual( true, APIWeather(syote).Contains("404") , "in method APIWeather, line 63");
			syote = "Jyvskylä";
			Assert.AreEqual( false, APIWeather(syote).Contains("200") , "in method APIWeather, line 65");
		}
		[Test]
		public  void TestCompare215()
		{
			int[] answers = {3, 20, 14};
			int[] dataF = {3, 45, 4};
			Assert.AreEqual( 1, Compare(answers, dataF) , "in method Compare, line 218");
			int[] answerss = {12, 41, 5};
			int[] data = {12, 41, 4};
			Assert.AreEqual( 2, Compare(answerss, data) , "in method Compare, line 221");
		}
	}
}

