/// Credit for this parser implementation belongs to https://github.com/gman4455/reminder-parser

using System;
using System.Linq;
using System.Collections.Generic;

namespace albert_extensions
{
	class Parser
	{
		static List<Keyword> Keywords = new Keyword[] {
			new Keyword(PhraseType.Action, "to"),
			new Keyword(PhraseType.Action, "about"),
			new Keyword(PhraseType.Time, "at"),
			new Keyword(PhraseType.Time, "on"),
			new Keyword(PhraseType.Time, "in"),
			new Keyword(PhraseType.Time, "today"),
			new Keyword(PhraseType.Time, "tomorrow"),
			new Keyword(PhraseType.Time, "next")
		}.ToList();

		public static Reminder Process(Queue<string> words)
		{
			var reminder = new Reminder();

			while (true)
			{
				if (words.Count == 0)
					break;
				if (Keywords.Select(k => k.Value).Contains(words.Peek()))
				{
					Phrase p = ProcessPhrase(words);
					if (p.Keyword.Type == PhraseType.Action)
					{
						string result = "";
						foreach (string s in p.Content)
						{
							result += s + " ";
						}
						result = result.TrimEnd();

						reminder.Title = result.First().ToString().ToUpper() + new string(result.Skip(1).ToArray());
					}
					else if (p.Keyword.Type == PhraseType.Time)
					{
						reminder.DueDate = ParseTime(p.Keyword.Value, p.Content);
					}
				}
				else
					words.Dequeue();
			}

			return reminder;
		}

		static Phrase ProcessPhrase(Queue<string> words)
		{
			string word = words.Dequeue();
			Keyword keyword = Keywords.First(k => k.Value == word);
			var content = new Queue<string>();

			while (words.Count > 0 && !Keywords.Where(k => k.Type != keyword.Type).Select(k => k.Value).Contains(words.Peek()))
			{
				content.Enqueue(words.Dequeue());
			}

			return new Phrase()
			{
				Keyword = keyword,
				Content = content
			};
		}

		static DateTime ParseTime(string keyword, Queue<string> words)
		{
			DateTime dateComponent = DateTime.Now;
			DateTime timeComponent = DateTime.Now;
			DateTime d = DateTime.Now;

			if (keyword == "in") // relative time. first parse number, then unit
			{
				string word = words.Dequeue();
				int number = 0;
				if (!int.TryParse(word, out number) && word.StartsWith("a", StringComparison.Ordinal))
					number = 1;
				string unit = words.Dequeue();
				unit = unit.TrimEnd('s');

				switch (unit)
				{
					case "minute":
						d = d.AddMinutes(number);
						break;
					case "hour":
						d = d.AddHours(number);
						break;
					case "day":
						d = d.AddDays(number);
						break;
					case "week":
						d = d.AddDays(number * 7);
						break;
					case "month":
						d = d.AddMonths(number);
						break;
					case "year":
						d = d.AddYears(number);
						break;
					default:
						d = d.AddHours(1);
						break;
				}
			}
			else
			{
				if (keyword == "on")
				{
					// try parse date string
					string content = ContentToNextKeyword(words);

					if (!DateTime.TryParse(content, out dateComponent))
					{
						// search for day of week
						dateComponent = GetNextDayOfWeek(content.ToLower());
					}

					if (words.Count > 0 && words.Peek() == "at")
					{
						words.Dequeue();

						timeComponent = ParseTimeAt(words);
					}
					else
					{
						// use default time of day
						timeComponent = DateTime.Parse("9:00 AM");
					}
				}
				else if (keyword == "at")
				{
					timeComponent = ParseTimeAt(words);
				}
				else
				{
					if (keyword == "today")
						dateComponent = DateTime.Today;
					else if (keyword == "tomorrow")
						dateComponent = DateTime.Today.AddDays(1);

					if (words.Count > 0 && words.Peek() == "at")
					{
						words.Dequeue();

						timeComponent = ParseTimeAt(words);
					}
					else
					{
						// use default time of day
						timeComponent = DateTime.Parse("9:00 AM");
					}
				}

				d = dateComponent.Date.Add(timeComponent.TimeOfDay);
			}

			return d;
		}

		static DateTime ParseTimeAt(Queue<string> words)
		{
			return DateTime.Parse(words.Dequeue());
		}

		static DateTime GetNextDayOfWeek(string day)
		{
			DayOfWeek dow;

			switch (day)
			{
				case "Sunday":
					dow = DayOfWeek.Sunday;
					break;
				case "Monday":
					dow = DayOfWeek.Monday;
					break;
				case "Tuesday":
					dow = DayOfWeek.Tuesday;
					break;
				case "Wednesday":
					dow = DayOfWeek.Wednesday;
					break;
				case "Thursday":
					dow = DayOfWeek.Thursday;
					break;
				case "Friday":
					dow = DayOfWeek.Friday;
					break;
				case "Saturday":
					dow = DayOfWeek.Saturday;
					break;
				default:
					dow = DateTime.Today.DayOfWeek;
					break;
			}

			int daysUntil = ((int)dow - (int)DateTime.Today.DayOfWeek + 7) % 7;

			return DateTime.Now.AddDays(daysUntil);
		}

		static string ContentToNextKeyword(Queue<string> words)
		{
			string content = "";

			while (words.Count > 0)
			{
				if (Keywords.Select(k => k.Value).Contains(words.Peek()))
					break;
				content += words.Dequeue() + " ";
			}

			content = content.Trim();

			return content;
		}
	}

	public class Reminder
	{
		public string Title { get; set; }
		public DateTime DueDate { get; set; }
	}

	public class Phrase
	{
		public Keyword Keyword { get; set; }
		public Queue<string> Content { get; set; }
	}

	public struct Keyword
	{
		public PhraseType Type { get; set; }
		public string Value { get; set; }

		public Keyword(PhraseType type, string value)
		{
			Type = type;
			Value = value;
		}
	}

	public enum PhraseType
	{
		Action,
		Time
	}
}
