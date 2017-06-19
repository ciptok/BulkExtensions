﻿using System;
using System.Collections;
using System.Collections.Generic;
#if !NETSTANDARD1_3
using System.Data;
#endif
using System.Data.Common;
using System.Linq;
#if EF6
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
#endif

namespace EntityFramework.BulkExtensions.Commons.Helpers
{
    internal class EnumerableDataReader : DbDataReader
    {
        private object[] _currentElement;
        private readonly IList<object[]> _collection;
        private readonly IList<string> _columns;
        private readonly IEnumerator _enumerator;
        private readonly IList<Guid> _columnGuids;

        internal EnumerableDataReader(IEnumerable<string> columns, IEnumerable<object[]> collection)
        {
            _columns = columns.ToList();
            _collection = collection.ToList();
            _enumerator = _collection.GetEnumerator();
            _enumerator.Reset();
            _columnGuids = new List<Guid>();
            foreach (var unused in _columns)
            {
                _columnGuids.Add(Guid.NewGuid());
            }
        }

#if !NETSTANDARD1_3

        public override void Close()
        {
        }

        public override DataTable GetSchemaTable()
        {
            return new DataTable();
        }

#endif

        public override bool NextResult()
        {
            var moved = _enumerator.MoveNext();
            if (moved)
                _currentElement = _enumerator.Current as object[];
            return moved;
        }

        public override bool Read()
        {
            var moved = _enumerator.MoveNext();
            if (moved)
                _currentElement = _enumerator.Current as object[];
            return moved;
        }

        public override int Depth => 0;
        public override bool IsClosed => false;
        public override int RecordsAffected => -1;

        public override bool GetBoolean(int ordinal)
        {
            return (bool) _currentElement[ordinal];
        }

        public override byte GetByte(int ordinal)
        {
            return (byte) _currentElement[ordinal];
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return (long) _currentElement[ordinal];
        }

        public override char GetChar(int ordinal)
        {
            return (char) _currentElement[ordinal];
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return (long) _currentElement[ordinal];
        }

        public override Guid GetGuid(int ordinal)
        {
            return _columnGuids[ordinal];
        }

        public override short GetInt16(int ordinal)
        {
            return (short) _currentElement[ordinal];
        }

        public override int GetInt32(int ordinal)
        {
            return (int) _currentElement[ordinal];
        }

        public override long GetInt64(int ordinal)
        {
            return (long) _currentElement[ordinal];
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime) _currentElement[ordinal];
        }

        public override string GetString(int ordinal)
        {
            return _currentElement[ordinal].ToString();
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (decimal) _currentElement[ordinal];
        }

        public override double GetDouble(int ordinal)
        {
            return (double) _currentElement[ordinal];
        }

        public override float GetFloat(int ordinal)
        {
            return (float) _currentElement[ordinal];
        }

        public override string GetName(int ordinal)
        {
            return _columns[ordinal];
        }

        public override int GetValues(object[] values)
        {
            return values.Length;
        }

        public override bool IsDBNull(int ordinal)
        {
            return _currentElement[ordinal] == null;
        }

        public override int FieldCount => _collection.First().Length;

        public override object this[int ordinal] => _currentElement;

        public override object this[string name]
        {
            get
            {
                var index = _columns.IndexOf(name);
                return index < 0 ? null : _collection[index];
            }
        }

        public override bool HasRows => _collection.Any();

        public override int GetOrdinal(string name)
        {
            return _columns.IndexOf(name);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _currentElement[ordinal].GetType().Name;
        }

        public override Type GetFieldType(int ordinal)
        {
            return _currentElement[ordinal].GetType();
        }

        public override object GetValue(int ordinal)
        {
#if EF6
            object value = _currentElement[ordinal];

            var dbgeo = value as DbGeography;
            if (dbgeo != null)
            {
                return SqlGeography.STGeomFromText(new SqlChars(dbgeo.WellKnownValue.WellKnownText),
                                                   dbgeo.CoordinateSystemId);
            }

            var dbgeom = value as DbGeometry;
            if (dbgeom != null)
            {
                return SqlGeometry.STGeomFromText(new SqlChars(dbgeom.WellKnownValue.WellKnownText), 
                                                  dbgeom.CoordinateSystemId);
            }

            return value;
#else
            return _currentElement[ordinal];
#endif
        }

        public override IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}