﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace KlonsLIB.Misc
{
    public static class Utils
    {

        public class TEventArgs<T> : EventArgs
        {
            public T Arg { get; set; }
            public TEventArgs(T arg)
            {
                Arg = arg;
            }
        }
        
        public static string GetMyFolder()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
            //return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static string GetMyFolderX()
        {
            string s1 = GetMyFolder().ToLower();
            string s2 = s1.ToLower();
            string[] ss = new[] { 
                "\\bin\\debug", 
                "\\bin\\release", 
                "\\bin\\x64\\debug", 
                "\\bin\\x86\\debug",
                "\\bin\\x64\\release",
                "\\bin\\x86\\release",
                "\\bin\\debug\\net8.0-windows",
                "\\bin\\release\\net8.0-windows",
                "\\bin\\x64\\debug\\net8.0-windows7.0",
                "\\bin\\x64\\release\\net8.0-windows7.0",
                "\\bin\\x86\\debug\\net8.0-windows7.0",
                "\\bin\\x86\\release\\net8.0-windows7.0",
                "\\bin\\x64\\debug\\net8.0-windows",
                "\\bin\\x64\\release\\net8.0-windows",
                "\\bin\\x86\\debug\\net8.0-windows",
                "\\bin\\x86\\release\\net8.0-windows",
            };
            var p1 = ss.FirstOrDefault(x => s2.EndsWith(x));
            if (p1 == null) return s1;
            var projrct_dir_name = s1.Substring(0, s1.Length - p1.Length);
            return projrct_dir_name;
        }

        public static string GetFileNameFromURL(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            int k = url.LastIndexOfAny("\\/".ToArray());
            if (k > -1) return url.Substring(k + 1);
            return url;
        }

        public static string RemoveExt(string filename)
        {
            int i, k = filename.Length - 1;
            char c;
            for (i = k; i >= 0; i--)
            {
                c = filename[i];
                if (c == '.') return filename.Substring(0, i);
                if (c == '\\' || c == '/') return filename;
            }
            return filename;
        }

        public static string GetExt(string filename)
        {
            int i, k = filename.Length - 1;
            char c;
            for (i = k; i >= 0; i--)
            {
                c = filename[i];
                if (c == '.') return filename.Substring(i + 1, k - i);
                if (c == '\\' || c == '/') return "";
            }
            return "";
        }

        public static string GetNextFile(string folder, string prefix, string ext)
        {
            string fnm = Path.Combine(folder, prefix);
            ext = "." + ext;
            int i = 1;
            string s;
            do
            {
                s = fnm + i + ext;
                i++;
            }
            while (File.Exists(s)) ;
            return s;
        }

        public static long GetFileSize(string filename)
        {
            try
            {
                FileInfo fileinfo = new FileInfo(filename);
                return fileinfo.Length;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static bool IN(int val, params int[] list)
        {
            if (list == null || list.Length == 0) return false;
            foreach(int v in list)
                if (v == val) return true;
            return false;
        }

        public static bool IN(string val, params string[] list)
        {
            if (list == null || list.Length == 0) return false;
            return list.Contains(val);
        }

        public static bool IsNOE(this string s) => string.IsNullOrEmpty(s);

        public static bool ContainsCI(this string s, string text)
        {
            return !s.IsNOE() && !text.IsNOE() && s.Contains(text, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithCI(this string s, string text)
        {
            return !s.IsNOE() && !text.IsNOE() && s.StartsWith(text, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StringToDate(string value, out DateTime date)
        {
            return DateTime.TryParseExact(value, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date);
        }

        public static DateTime? StringToDate(string value)
        {
            DateTime dt;
            if (DateTime.TryParseExact(value, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
            {
                return dt;
            }
            return null;
        }

        public static string DateToString(DateTime date)
        {
            return date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        public static string DateNToString(DateTime? date)
        {
            if (date == null) return "";
            return date.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        public static string DateToLongString(DateTime date)
        {
            return $"{date.Year}. gada {date.Day}. {MonthNames[date.Month - 1]}";
        }

        public static string ReformatDateString(string date)
        {
            DateTime dt;
            if (!StringToDate(date, out dt)) return date;
            return DateToString(dt);
        }

        public static string[] MonthNames = new string[]
        {
            "janvāris", "februāris", "marts", "aprīlis", "maijs", "jūnijs",
            "jūlijs", "augusts", "septembris", "oktobris", "novembris", "decembris"
        };

        public static string[] MonthNamesAcc = new string[]
        {
            "janvāri", "februāri", "martu", "aprīli", "maiju", "jūniju",
            "jūliju", "augustu", "septembri", "oktobri", "novembri", "decembri"
        };

        public static int DaysInMonth(this DateTime dt)
        {
            var dt1 = new DateTime(dt.Year, dt.Month, 1);
            return (dt1.AddMonths(1) - dt1).Days;
        }
        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
        }
        public static bool IsLastOfYear(this DateTime dt)
        {
            return dt.Month == 12 && dt.Day == 31;
        }
        public static DateTime LastDayOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 12, 31);
        }
        public static bool IsBetween(this DateTime dt, DateTime dt1, DateTime dt2)
        {
            return dt >= dt1 && dt <= dt2;
        }

        public static void AddMonths(ref int yr, ref int mt, int addmt)
        {
            yr += (addmt - 1) / 12;
            mt += (addmt - 1) % 12 + 1;
            if (mt < 1)
            {
                yr--;
                mt = 12 - mt;
            }
            else if (mt > 12)
            {
                yr++;
                mt = mt - 12;
            }
        }

        public static int MonthDiff(DateTime dtsmall, DateTime dtbig)
        {
            return (dtbig.Year - dtsmall.Year) * 12 + dtbig.Month - dtsmall.Month;
        }

        public static bool StringEndsWith(string s, string end)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(end)) return false;
            int ls = s.Length, lend = end.Length;
            if (ls < lend) return false;
            return s.Substring(ls - lend) == end;
        }

        /// <summary>
        ///     Returns "" when s is null
        /// </summary>
        public static string Nz(this string s)
        {
            return s == null ? "" : s;
        }

        /// <summary>
        ///     Returns null when s = ""
        /// </summary>
        public static string Zn(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }
        public static string LeftMax(this string s, int count)
        {
            if (s == null) return null;
            if (s.Length < count) return s;
            return s.Substring(0, count);
        }

        public static bool IsGreaterThan(this string s1, string s2)
        {
            if (s1 == null || s2 == null) return false;
            return string.Compare(s1, s2, true) > 0;
        }

        public static bool IsLessThan(this string s1, string s2)
        {
            if (s1 == null || s2 == null) return false;
            return string.Compare(s1, s2, true) < 0;
        }

        public static int FontSizeX(this Font font)
        {
            return (int) Math.Round(font.SizeInPoints, 0);
        }

        public static bool IsTheSameArray(this int[] arr1, int[] arr2)
        {
            if (arr1 == null || arr2 == null || arr1.Length != arr2.Length) return false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i]) return false;
            }
            return true;
        }
        public static int IndexOf(this string[] arr, string value)
        {
            if (arr == null || arr.Length == 0) return -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value) return i;
            }
            return -1;
        }
        public static int IndexOf(this object[] arr, object value)
        {
            if (arr == null || arr.Length == 0) return -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (object.ReferenceEquals(arr[i], value)) return i;
            }
            return -1;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
            foreach (T item in source)
                action(item);
        }

        public static IEnumerable<TSource> DistinctByForOrdered<TSource, TKey>(this IEnumerable<TSource> source,
                  Func<TSource, TSource, bool> fequals)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (fequals == null) throw new ArgumentNullException("fequals");
            bool firsrfound = false;
            TSource prevobj = default(TSource);
            foreach (var element in source)
            {
                if (!firsrfound)
                {
                    firsrfound = true;
                    prevobj = element;
                    yield return element;
                    continue;
                }
                if (fequals(element, prevobj)) continue;
                yield return element;
            }
        }

        public static IEnumerable<TSource> DistinctByForOrdered<TSource, TKey>(this IEnumerable<TSource> source,
                  Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            bool firsrfound = false;
            TSource prevobj = default(TSource);
            foreach (var element in source)
            {
                if (!firsrfound)
                {
                    firsrfound = true;
                    prevobj = element;
                    yield return element;
                    continue;
                }
                if (object.Equals(keySelector(element), keySelector(prevobj))) continue;
                yield return element;
            }
        }

        public static IEnumerable<TSource> DistinctForOrdered<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            bool firsrfound = false;
            TSource prevobj = default(TSource);
            foreach (var element in source)
            {
                if (!firsrfound)
                {
                    firsrfound = true;
                    prevobj = element;
                    yield return element;
                    continue;
                }
                if (object.Equals(element, prevobj)) continue;
                yield return element;
            }
        }

        public static TSource WithMaxOrDefault<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IComparable> fgetcomparable, TSource defaultvalue = default(TSource))
        {
            if (source == null) throw new ArgumentNullException("source");
            if (fgetcomparable == null) throw new ArgumentNullException("fgetcomparable");

            bool firstfound = false;
            TSource maxobj = defaultvalue;
            foreach (var element in source)
            {
                if (element == null) continue;
                if (!firstfound)
                {
                    firstfound = true;
                    maxobj = element;
                    continue;
                }
                var comp1 = fgetcomparable(element);
                var comp2 = fgetcomparable(maxobj);
                if (comp1.CompareTo(comp2) > 0)
                    maxobj = element;
            }
            if (firstfound) return maxobj;
            return defaultvalue;
        }

        public static TSource WithMinOrDefault<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IComparable> fgetcomparable, TSource defaultvalue = default(TSource))
        {
            if (source == null) throw new ArgumentNullException("source");
            if (fgetcomparable == null) throw new ArgumentNullException("fgetcomparable");

            bool firstfound = false;
            TSource maxobj = defaultvalue;
            foreach (var element in source)
            {
                if (element == null) continue;
                if (!firstfound)
                {
                    firstfound = true;
                    maxobj = element;
                    continue;
                }
                var comp1 = fgetcomparable(element);
                var comp2 = fgetcomparable(maxobj);
                if (comp1.CompareTo(comp2) < 0)
                    maxobj = element;
            }
            if (firstfound) return maxobj;
            return defaultvalue;
        }

        //for example: can remove Click events from ToolStripMenuItem (eventname: EventClick)
        public static void ClearEventInvocations(object obj, string eventName)
        {
            PropertyInfo prop = GetProp(obj.GetType(), "Events");
            if (prop == null) return;
            EventHandlerList el = prop.GetValue(obj, null) as EventHandlerList;
            if (el == null) return;

            FieldInfo field = GetField(obj.GetType(), eventName);
            if (field == null) return;
            object o = field.GetValue(obj);
            Delegate d = el[o];
            //el.RemoveHandler(o, el[o]);
            el[o] = null;
        }

        public static bool HasProperty(object obj, string propName)
        {
            PropertyInfo prop = GetProp(obj.GetType(), propName);
            return prop != null;
        }

        public static bool SetProperty(object obj, string propName, object value)
        {
            PropertyInfo prop = GetProp(obj.GetType(), propName);
            if (prop == null) return false;
            try
            {
                prop.SetValue(obj, value, null);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static object GetProperty(object obj, string propName)
        {
            PropertyInfo prop = GetProp(obj.GetType(), propName);
            if (prop == null) return null;
            try
            {
                return prop.GetValue(obj, null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool GetPublicPropertyValue(object obj, string propname, out object value)
        {
            value = null;
            if (obj == null || string.IsNullOrEmpty(propname)) return false;

            var prop = TypeDescriptor.GetProperties(obj).Find(propname, false);
            //var prop = obj.GetType().GetProperty(propname);

            if (prop == null)
            {
                var prop2 = obj.GetType().GetProperty(propname);
                if (prop != null)
                {
                    Debug.Print("GetProperty shit");
                    throw new Exception("GetProperty shit");
                }

                return false;
            }

            try
            {
                value = prop.GetValue(obj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int FindValue(IList list, string propname, object value)
        {
            if (list == null || list.Count == 0 || string.IsNullOrEmpty(propname)) return -1;
            object o1;
            for (int i = 0; i < list.Count; i++)
            {
                var o2 = list[i];
                if (o2 == null) continue;
                if (!GetPublicPropertyValue(list[i], propname, out o1)) continue;
                if (object.Equals(o1, value))
                    return i;
            }
            return -1;
        }

        public static object CallMethod(object obj, string methodName)
        {
            MethodInfo method = GetMethod(obj.GetType(), methodName);
            if (method == null) return null;
            return method.Invoke(obj, null);
        }
        public static object CallMethod(object obj, string methodName, params object[] args)
        {
            MethodInfo method = GetMethod(obj.GetType(), methodName);
            if (method == null) return null;
            return method.Invoke(obj, args);
        }

        public static EventInfo GetEvent(Type type, string eventName)
        {
            EventInfo evinfo = null;
            while (type != null)
            {
                evinfo = type.GetEvent(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (evinfo != null) break;

                type = type.BaseType;
            }
            return evinfo;
        }

        public static PropertyInfo GetProp(this Type type, string propName)
        {
            PropertyInfo prop = null;
            while (type != null)
            {
                prop = type.GetProperty(propName, BindingFlags.Public | 
                    BindingFlags.Instance | BindingFlags.Static | 
                    BindingFlags.NonPublic);
                if (prop != null) break;

                type = type.BaseType;
            }
            return prop;
        }

        public static FieldInfo GetField(Type type, string fieldName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                field = type.GetField(fieldName, BindingFlags.Public | 
                    BindingFlags.Static | BindingFlags.Instance | 
                    BindingFlags.NonPublic);
                if (field != null) break;

                type = type.BaseType;
            }
            return field;
        }

        public static MethodInfo GetMethod(Type type, string methodName)
        {
            MethodInfo method = null;
            while (type != null)
            {
                method = type.GetMethod(methodName, BindingFlags.Public | 
                    BindingFlags.Static | BindingFlags.Instance | 
                    BindingFlags.NonPublic);
                if (method != null) break;

                type = type.BaseType;
            }
            return method;
        }

        private static readonly Dictionary<Type, XmlSerializer> _xmlSerializerCache = new Dictionary<Type, XmlSerializer>();

        public static XmlSerializer CreateDefaultXmlSerializer(Type type)
        {
            XmlSerializer serializer;
            if (_xmlSerializerCache.TryGetValue(type, out serializer))
            {
                return serializer;
            }
            else
            {
                var importer = new XmlReflectionImporter();
                var mapping = importer.ImportTypeMapping(type, null, null);
                serializer = new XmlSerializer(mapping);
                return _xmlSerializerCache[type] = serializer;
            }
        }

        public static T LoadDataFromXMLOrCreateEmpty<T>(string filename) where T : new()
        {
            T data = LoadDataFromXML<T>(filename);
            if (data != null) return data;
            return new T();
        }

        public static T LoadDataFromXML<T>(string filename) where T : new()
        {
            T data = default;
            if (!File.Exists(filename)) return data;
            XmlSerializer xs = null;
            FileStream fs = null;
            try
            {
                xs = Utils.CreateDefaultXmlSerializer(typeof(T));
                fs = new FileStream(filename, FileMode.Open);
                data = (T)xs.Deserialize(fs);
                return data;
            }
            catch (Exception)
            {
                return default;
            }
            finally
            {
                fs?.Close();
            }
        }

        public static bool SaveDataToXML<T>(T data, string filename)
        {
            XmlSerializer xs = Utils.CreateDefaultXmlSerializer(typeof(T));
            TextWriter wr = null;
            try
            {
                wr = new StreamWriter(filename);
                xs.Serialize(wr, data);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (wr != null) wr.Close();
            }
            return true;
        }

        public static string AsString(this object o)
        {
            if (o == null || o == DBNull.Value) return null;
            return (string) o;
        }

        public static int AsInt(this object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            return (int)o;
        }
        public static decimal AsDecimal(this object o)
        {
            if (o == null || o == DBNull.Value) return 0.00M;
            return (decimal)o;
        }
        public static float AsFloat(this object o)
        {
            if (o == null || o == DBNull.Value) return 0.00f;
            return (float)o;
        }
        public static int DayOfWeekA(this DateTime dt)
        {
            int wd = (int)dt.DayOfWeek;
            if (wd == 0) wd = 7;
            return wd;
        }

        public static Type GetFirstTypeX(Type type)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(t => t.IsSubclassOf(type)).FirstOrDefault();
        }

        public static bool DGVParseDateCell(DataGridViewCellParsingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return false;
            if (!(e.Value is string)) return false;
            string s = (string) e.Value;
            if (string.IsNullOrEmpty(s)) return false;
            DateTime dt;
            if (!Utils.StringToDate(s, out dt)) return false;
            e.Value = dt;
            e.ParsingApplied = true;
            return true;
        }

        public static Dictionary<TKey, List<T>> BreakListInGroups<T, TKey>(
            IEnumerable<T> list, Func<T, TKey> getkey)
        {
            var dic = new Dictionary<TKey, List<T>>();
            List<T> list_k;
            foreach (var d in list)
            {
                TKey key = getkey(d);
                if (!dic.TryGetValue(key, out list_k))
                {
                    list_k = new List<T>();
                    dic[key] = list_k;
                }
                list_k.Add(d);
            }
            return dic;
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        public static bool IsNullableType(this Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }
    

        //for small differences
        public static decimal MakeExactSum<T>(decimal sum, IEnumerable<T> list,
            Func<T, decimal> fgetval, Action<T, decimal> fsetval)
        {
            decimal cursum = list.Sum(d => fgetval(d));
            if (cursum == sum) return cursum;
            decimal addsumd = cursum < sum ? 0.01M : -0.01M;
            if (cursum == 0.0M)
            {
                while (true)
                {
                    foreach (var pfx in list)
                    {
                        decimal curval = fgetval(pfx);
                        fsetval(pfx, curval + addsumd);
                        cursum += addsumd;
                        if (cursum == sum) return cursum;
                    }
                }
            }
            while (true)
            {
                foreach (var pfx in list)
                {
                    decimal curval = fgetval(pfx);
                    if (curval == 0.0M) continue;
                    fsetval(pfx, curval + addsumd);
                    cursum += addsumd;
                    if (cursum == sum) return cursum;
                }
            }
        }
		
        public static void SetLabelColumnWidth(PropertyGrid grid, int width)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");
            Control view = (Control)grid.GetType().GetField("_gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);
            FieldInfo fi = view.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            fi.SetValue(view, width);
            view.Invalidate();
        }

        public static void SetLabelColumnWidth2(this PropertyGrid grid, int width)
        {
            FieldInfo fi = grid?.GetType().GetField("_gridView", BindingFlags.Instance | BindingFlags.NonPublic);
            Control view = fi?.GetValue(grid) as Control;
            MethodInfo mi = view?.GetType().GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);
            mi?.Invoke(view, new object[] { width });		
		}

        public static Predicate<T> JoinPredicatesWitnAnd<T>(Predicate<T> predicate1, Predicate<T> predicate2)
        {
            return x => predicate1(x) && predicate2(x);
        }

        public static Predicate<T> JoinPredicatesWitnAnd<T>(bool defaultresult, params Predicate<T>[] predicates)
        {
            Predicate<T> ret = null;
            foreach (var pred in predicates)
            {
                if (pred == null) continue;
                if (ret == null)
                {
                    ret = pred;
                    continue;
                }
                ret = JoinPredicatesWitnAnd(ret, pred);
            }
            if (ret == null)
                ret = x => defaultresult;
            return ret;
        }

        public static T NewItem<T>(this BindingList<T> bl) where T : new() => new();

    }
}
