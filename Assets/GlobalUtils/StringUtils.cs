using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tiggly{

public static class StringUtils {
	
	public static int nbOccurences (this string s, char c)
	{
		int res = 0;
		foreach(char ch in s)
			if (ch == c)
				res++;
		return res;
	}
	
	public static List<int> Occurences (this string s, char c)
	{
		List<int> res = new List<int>();
		for(int i = 0 ; i < s.Length ; i++)
		{
			char ch = s[i];
			if (ch == c)
				res.Add(i);
		}
		return res;
	}
	
	public static List<int> IndexesOf (this string s, char c)
	{
		return s.Occurences(c);
	}
	
	public static char ToLower (this char c)
	{
		return c.ToString().ToLower()[0];
	}
	
	public static char ToUpper (this char c)
	{
		return c.ToString().ToUpper()[0];
	}
	
	public static bool IsVowel (this char c)
	{
		c = c.ToLower();
		return (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u');
	}

}

}
