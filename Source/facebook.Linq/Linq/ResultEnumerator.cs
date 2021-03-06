﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


using System.Diagnostics;
using System.Linq.Expressions;

namespace facebook.Linq
{


	public class FqlQueryResultEnumerable<TRowType, TResult> : IEnumerable<TResult>, IEnumerator<TResult>
	{
		public FqlQueryResultEnumerable(string responseXml, Expression<Func<TRowType, TResult>> select)
		{
			ResponseXml = responseXml;
			Reader = new FacebookDataReader(ResponseXml);
			if (select != null)
				Selector = select.Compile();
		}
		Func<TRowType, TResult> Selector;

		string ResponseXml;
		FacebookDataReader Reader;

		#region IEnumerable<T> Members
		public IEnumerator<TResult> GetEnumerator()
		{
			while (Reader.Read())
			{
				TRowType item = Activator.CreateInstance<TRowType>();
				var typeData = KnownTypeData.GetTypeData(typeof(TRowType));
				foreach (var p in typeData.Properties)
				{
					var realPropertyName = p.Key;
					var fqlFieldName = p.Value.FqlFieldName;
					var propInfo = p.Value.PropertyInfo;
					var value = Reader[fqlFieldName];
					if(value!=null)
						propInfo.SetValue(item, value, null);					
				}
				if (Selector != null)
					yield return Selector(item);
				else
					yield return (TResult)(object)item;//TODO:
			}
		}			

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IEnumerator<T> Members

		public TResult Current
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerator Members

		object IEnumerator.Current
		{
			get { throw new NotImplementedException(); }
		}

		public bool MoveNext()
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
