using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	// Token: 0x02000020 RID: 32
	public static class MiniJSON
	{
		// Token: 0x0600017E RID: 382 RVA: 0x0000E900 File Offset: 0x0000CB00
		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return MiniJSON.Parser.Parse(json);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000E910 File Offset: 0x0000CB10
		public static string Serialize(object obj)
		{
			return MiniJSON.Serializer.Serialize(obj);
		}

		// Token: 0x02000021 RID: 33
		private sealed class Parser : IDisposable
		{
			// Token: 0x06000180 RID: 384 RVA: 0x0000E918 File Offset: 0x0000CB18
			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			// Token: 0x06000181 RID: 385 RVA: 0x0000E92C File Offset: 0x0000CB2C
			public static bool IsWordBreak(char c)
			{
				return char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;
			}

			// Token: 0x06000182 RID: 386 RVA: 0x0000E950 File Offset: 0x0000CB50
			public static object Parse(string jsonString)
			{
				object result;
				using (MiniJSON.Parser parser = new MiniJSON.Parser(jsonString))
				{
					result = parser.ParseValue();
				}
				return result;
			}

			// Token: 0x06000183 RID: 387 RVA: 0x0000E9A0 File Offset: 0x0000CBA0
			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			// Token: 0x06000184 RID: 388 RVA: 0x0000E9B4 File Offset: 0x0000CBB4
			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				this.json.Read();
				for (;;)
				{
					MiniJSON.Parser.TOKEN nextToken = this.NextToken;
					switch (nextToken)
					{
					case MiniJSON.Parser.TOKEN.NONE:
						goto IL_37;
					default:
						if (nextToken != MiniJSON.Parser.TOKEN.COMMA)
						{
							string text = this.ParseString();
							if (text == null)
							{
								goto Block_2;
							}
							if (this.NextToken != MiniJSON.Parser.TOKEN.COLON)
							{
								goto Block_3;
							}
							this.json.Read();
							dictionary[text] = this.ParseValue();
						}
						break;
					case MiniJSON.Parser.TOKEN.CURLY_CLOSE:
						return dictionary;
					}
				}
				IL_37:
				return null;
				Block_2:
				return null;
				Block_3:
				return null;
			}

			// Token: 0x06000185 RID: 389 RVA: 0x0000EA40 File Offset: 0x0000CC40
			private List<object> ParseArray()
			{
				List<object> list = new List<object>();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					MiniJSON.Parser.TOKEN nextToken = this.NextToken;
					MiniJSON.Parser.TOKEN token = nextToken;
					switch (token)
					{
					case MiniJSON.Parser.TOKEN.SQUARED_CLOSE:
						flag = false;
						break;
					default:
					{
						if (token == MiniJSON.Parser.TOKEN.NONE)
						{
							return null;
						}
						object item = this.ParseByToken(nextToken);
						list.Add(item);
						break;
					}
					case MiniJSON.Parser.TOKEN.COMMA:
						break;
					}
				}
				return list;
			}

			// Token: 0x06000186 RID: 390 RVA: 0x0000EABC File Offset: 0x0000CCBC
			private object ParseValue()
			{
				MiniJSON.Parser.TOKEN nextToken = this.NextToken;
				return this.ParseByToken(nextToken);
			}

			// Token: 0x06000187 RID: 391 RVA: 0x0000EAD8 File Offset: 0x0000CCD8
			private object ParseByToken(MiniJSON.Parser.TOKEN token)
			{
				switch (token)
				{
				case MiniJSON.Parser.TOKEN.CURLY_OPEN:
					return this.ParseObject();
				case MiniJSON.Parser.TOKEN.SQUARED_OPEN:
					return this.ParseArray();
				case MiniJSON.Parser.TOKEN.STRING:
					return this.ParseString();
				case MiniJSON.Parser.TOKEN.NUMBER:
					return this.ParseNumber();
				case MiniJSON.Parser.TOKEN.TRUE:
					return true;
				case MiniJSON.Parser.TOKEN.FALSE:
					return false;
				case MiniJSON.Parser.TOKEN.NULL:
					return null;
				}
				return null;
			}

			// Token: 0x06000188 RID: 392 RVA: 0x0000EB50 File Offset: 0x0000CD50
			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() == -1)
					{
						break;
					}
					char nextChar = this.NextChar;
					char c = nextChar;
					if (c != '"')
					{
						if (c != '\\')
						{
							stringBuilder.Append(nextChar);
						}
						else if (this.json.Peek() == -1)
						{
							flag = false;
						}
						else
						{
							nextChar = this.NextChar;
							char c2 = nextChar;
							switch (c2)
							{
							case 'n':
								stringBuilder.Append('\n');
								break;
							default:
								if (c2 != '"' && c2 != '/' && c2 != '\\')
								{
									if (c2 != 'b')
									{
										if (c2 == 'f')
										{
											stringBuilder.Append('\f');
										}
									}
									else
									{
										stringBuilder.Append('\b');
									}
								}
								else
								{
									stringBuilder.Append(nextChar);
								}
								break;
							case 'r':
								stringBuilder.Append('\r');
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case 'u':
							{
								char[] array = new char[4];
								for (int i = 0; i < 4; i++)
								{
									array[i] = this.NextChar;
								}
								stringBuilder.Append((char)Convert.ToInt32(new string(array), 16));
								break;
							}
							}
						}
					}
					else
					{
						flag = false;
					}
				}
				return stringBuilder.ToString();
			}

			// Token: 0x06000189 RID: 393 RVA: 0x0000ECE8 File Offset: 0x0000CEE8
			private object ParseNumber()
			{
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long num;
					long.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
					return num;
				}
				double num2;
				double.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
				return num2;
			}

			// Token: 0x0600018A RID: 394 RVA: 0x0000ED40 File Offset: 0x0000CF40
			private void EatWhitespace()
			{
				while (char.IsWhiteSpace(this.PeekChar))
				{
					this.json.Read();
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
			}

			// Token: 0x1700002C RID: 44
			// (get) Token: 0x0600018B RID: 395 RVA: 0x0000ED7C File Offset: 0x0000CF7C
			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x0600018C RID: 396 RVA: 0x0000ED90 File Offset: 0x0000CF90
			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x0600018D RID: 397 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (!MiniJSON.Parser.IsWordBreak(this.PeekChar))
					{
						stringBuilder.Append(this.NextChar);
						if (this.json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x0600018E RID: 398 RVA: 0x0000EDF8 File Offset: 0x0000CFF8
			private MiniJSON.Parser.TOKEN NextToken
			{
				get
				{
					this.EatWhitespace();
					if (this.json.Peek() == -1)
					{
						return MiniJSON.Parser.TOKEN.NONE;
					}
					char peekChar = this.PeekChar;
					switch (peekChar)
					{
					case '"':
						return MiniJSON.Parser.TOKEN.STRING;
					default:
						switch (peekChar)
						{
						case '[':
							return MiniJSON.Parser.TOKEN.SQUARED_OPEN;
						default:
						{
							switch (peekChar)
							{
							case '{':
								return MiniJSON.Parser.TOKEN.CURLY_OPEN;
							case '}':
								this.json.Read();
								return MiniJSON.Parser.TOKEN.CURLY_CLOSE;
							}
							string nextWord = this.NextWord;
							switch (nextWord)
							{
							case "false":
								return MiniJSON.Parser.TOKEN.FALSE;
							case "true":
								return MiniJSON.Parser.TOKEN.TRUE;
							case "null":
								return MiniJSON.Parser.TOKEN.NULL;
							}
							return MiniJSON.Parser.TOKEN.NONE;
						}
						case ']':
							this.json.Read();
							return MiniJSON.Parser.TOKEN.SQUARED_CLOSE;
						}
						break;
					case ',':
						this.json.Read();
						return MiniJSON.Parser.TOKEN.COMMA;
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return MiniJSON.Parser.TOKEN.NUMBER;
					case ':':
						return MiniJSON.Parser.TOKEN.COLON;
					}
				}
			}

			// Token: 0x04000094 RID: 148
			private const string WORD_BREAK = "{}[],:\"";

			// Token: 0x04000095 RID: 149
			private StringReader json;

			// Token: 0x02000022 RID: 34
			private enum TOKEN
			{
				// Token: 0x04000098 RID: 152
				NONE,
				// Token: 0x04000099 RID: 153
				CURLY_OPEN,
				// Token: 0x0400009A RID: 154
				CURLY_CLOSE,
				// Token: 0x0400009B RID: 155
				SQUARED_OPEN,
				// Token: 0x0400009C RID: 156
				SQUARED_CLOSE,
				// Token: 0x0400009D RID: 157
				COLON,
				// Token: 0x0400009E RID: 158
				COMMA,
				// Token: 0x0400009F RID: 159
				STRING,
				// Token: 0x040000A0 RID: 160
				NUMBER,
				// Token: 0x040000A1 RID: 161
				TRUE,
				// Token: 0x040000A2 RID: 162
				FALSE,
				// Token: 0x040000A3 RID: 163
				NULL
			}
		}

		// Token: 0x02000023 RID: 35
		private sealed class Serializer
		{
			// Token: 0x0600018F RID: 399 RVA: 0x0000EF70 File Offset: 0x0000D170
			private Serializer()
			{
				this.builder = new StringBuilder();
			}

			// Token: 0x06000190 RID: 400 RVA: 0x0000EF84 File Offset: 0x0000D184
			public static string Serialize(object obj)
			{
				MiniJSON.Serializer serializer = new MiniJSON.Serializer();
				serializer.SerializeValue(obj, 1);
				return serializer.builder.ToString();
			}

			// Token: 0x06000191 RID: 401 RVA: 0x0000EFAC File Offset: 0x0000D1AC
			private void SerializeValue(object value, int indentationLevel)
			{
				string str;
				IList anArray;
				IDictionary obj;
				if (value == null)
				{
					this.builder.Append("null");
				}
				else if ((str = (value as string)) != null)
				{
					this.SerializeString(str);
				}
				else if (value is bool)
				{
					this.builder.Append((!(bool)value) ? "false" : "true");
				}
				else if ((anArray = (value as IList)) != null)
				{
					this.SerializeArray(anArray, indentationLevel);
				}
				else if ((obj = (value as IDictionary)) != null)
				{
					this.SerializeObject(obj, indentationLevel);
				}
				else if (value is char)
				{
					this.SerializeString(new string((char)value, 1));
				}
				else
				{
					this.SerializeOther(value);
				}
			}

			// Token: 0x06000192 RID: 402 RVA: 0x0000F084 File Offset: 0x0000D284
			private void SerializeObject(IDictionary obj, int indentationLevel)
			{
				bool flag = true;
				this.builder.Append('{');
				this.builder.Append('\n');
				for (int i = 0; i < indentationLevel; i++)
				{
					this.builder.Append('\t');
				}
				foreach (object obj2 in obj.Keys)
				{
					if (!flag)
					{
						this.builder.Append(',');
						this.builder.Append('\n');
						for (int j = 0; j < indentationLevel; j++)
						{
							this.builder.Append('\t');
						}
					}
					this.SerializeString(obj2.ToString());
					this.builder.Append(':');
					indentationLevel++;
					this.SerializeValue(obj[obj2], indentationLevel);
					indentationLevel--;
					flag = false;
				}
				this.builder.Append('\n');
				for (int k = 0; k < indentationLevel - 1; k++)
				{
					this.builder.Append('\t');
				}
				this.builder.Append('}');
			}

			// Token: 0x06000193 RID: 403 RVA: 0x0000F1E4 File Offset: 0x0000D3E4
			private void SerializeArray(IList anArray, int indentationLevel)
			{
				this.builder.Append('[');
				bool flag = true;
				for (int i = 0; i < anArray.Count; i++)
				{
					object value = anArray[i];
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeValue(value, indentationLevel);
					flag = false;
				}
				this.builder.Append(']');
			}

			// Token: 0x06000194 RID: 404 RVA: 0x0000F24C File Offset: 0x0000D44C
			private void SerializeString(string str)
			{
				this.builder.Append('"');
				foreach (char c in str.ToCharArray())
				{
					char c2 = c;
					switch (c2)
					{
					case '\b':
						this.builder.Append("\\b");
						break;
					case '\t':
						this.builder.Append("\\t");
						break;
					case '\n':
						this.builder.Append("\\n");
						break;
					default:
						if (c2 != '"')
						{
							if (c2 != '\\')
							{
								int num = Convert.ToInt32(c);
								if (num >= 32 && num <= 126)
								{
									this.builder.Append(c);
								}
								else
								{
									this.builder.Append("\\u");
									this.builder.Append(num.ToString("x4"));
								}
							}
							else
							{
								this.builder.Append("\\\\");
							}
						}
						else
						{
							this.builder.Append("\\\"");
						}
						break;
					case '\f':
						this.builder.Append("\\f");
						break;
					case '\r':
						this.builder.Append("\\r");
						break;
					}
				}
				this.builder.Append('"');
			}

			// Token: 0x06000195 RID: 405 RVA: 0x0000F3C0 File Offset: 0x0000D5C0
			private void SerializeOther(object value)
			{
				if (value is float)
				{
					this.builder.Append(((float)value).ToString("R", CultureInfo.InvariantCulture));
				}
				else if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong)
				{
					this.builder.Append(value);
				}
				else if (value is double || value is decimal)
				{
					this.builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
				}
				else if (value is Vector2)
				{
					Vector2 vector = (Vector2)value;
					this.builder.Append(string.Concat(new string[]
					{
						"\"(",
						vector.x.ToString("R", CultureInfo.InvariantCulture),
						",",
						vector.y.ToString("R", CultureInfo.InvariantCulture),
						")\""
					}));
				}
				else if (value is Vector3)
				{
					Vector3 vector2 = (Vector3)value;
					this.builder.Append(string.Concat(new string[]
					{
						"\"(",
						vector2.x.ToString("R", CultureInfo.InvariantCulture),
						",",
						vector2.y.ToString("R", CultureInfo.InvariantCulture),
						",",
						vector2.z.ToString("R", CultureInfo.InvariantCulture),
						")\""
					}));
				}
				else if (value is Vector4)
				{
					Vector4 vector3 = (Vector4)value;
					this.builder.Append(string.Concat(new string[]
					{
						"\"(",
						vector3.x.ToString("R", CultureInfo.InvariantCulture),
						",",
						vector3.y.ToString("R", CultureInfo.InvariantCulture),
						",",
						vector3.z.ToString("R", CultureInfo.InvariantCulture),
						",",
						vector3.w.ToString("R", CultureInfo.InvariantCulture),
						")\""
					}));
				}
				else if (value is Quaternion)
				{
					Quaternion quaternion = (Quaternion)value;
					this.builder.Append(string.Concat(new string[]
					{
						"\"(",
						quaternion.x.ToString("R", CultureInfo.InvariantCulture),
						",",
						quaternion.y.ToString("R", CultureInfo.InvariantCulture),
						",",
						quaternion.z.ToString("R", CultureInfo.InvariantCulture),
						",",
						quaternion.w.ToString("R", CultureInfo.InvariantCulture),
						")\""
					}));
				}
				else
				{
					this.SerializeString(value.ToString());
				}
			}

			// Token: 0x040000A4 RID: 164
			private StringBuilder builder;
		}
	}
}
