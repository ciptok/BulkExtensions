﻿using System;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.BulkExtensions;
using UnitTests.EF6.Database;
using UnitTests.EF6.Helpers;
using UnitTests.EF6.Model;
using Xunit;
using System.Data.Entity.Spatial;

namespace UnitTests.EF6.BulkInsertTests
{
    public class EF6_SimpleEntityBulkInsert : IDisposable
    {
        private readonly TestDatabase _context;
        private readonly IList<SimpleModel> _collection;

        public EF6_SimpleEntityBulkInsert()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            _context = new TestDatabase();
            ClearTable();
            _collection = new List<SimpleModel>();
            for (var i = 0; i < 10; i++)
            {
                _collection.Add(new SimpleModel
                {
                    StringProperty = Helper.RandomString(10),
                    IntValue = Helper.RandomInt(),
                    DateTime = DateTime.Today,
                    Type = Helper.RandomEnum(),
                    DoubleValue = Helper.RandomDouble(1, 10),
                    LatitudeLongitude = DbGeography.PointFromText(string.Format("POINT({0} {1})", 23, 16), 4326)
                });
            }
        }

        #region No Options Set

        [Fact]
        public void TestAffectedRowsCount()
        {
            var rowsCount = _context.BulkInsert(_collection);
            Assert.Equal(rowsCount, _collection.Count);
        }

        [Fact]
        public void TestInsertedEntitiesCount()
        {
            _context.BulkInsert(_collection);
            var simpleModels = _context.SimpleModel.ToList();
            Assert.Equal(simpleModels.Count, _collection.Count);
        }

        [Fact]
        public void TestInsertValuesSavedCorrectly()
        {
            _context.BulkInsert(_collection);
            var simpleModels = _context.SimpleModel
                .OrderBy(model => model.Id)
                .ToList();

            Assert.Equal(simpleModels.Count, _collection.Count);

            for (var i = 0; i < _collection.Count; i++)
            {
                var entity = _collection[i];
                var saved = simpleModels[i];
                Assert.NotEqual(entity.Id, saved.Id);
                Assert.Equal(entity.DateTime, saved.DateTime);
                Assert.Equal(entity.DoubleValue, saved.DoubleValue);
                Assert.Equal(entity.IntValue, saved.IntValue);
                Assert.Equal(entity.Type, saved.Type);
                Assert.Equal(entity.StringProperty, saved.StringProperty);
                Assert.Equal(entity.LatitudeLongitude.SpatialEquals(saved.LatitudeLongitude), true);
            }
        }

        #endregion

        #region Output Indentity Set

        [Fact]
        public void TestAffectedRowsCount_OutputIdentity()
        {
            var rowsCount = _context.BulkInsert(_collection, InsertOptions.OutputIdentity);
            Assert.Equal(rowsCount, _collection.Count);
        }

        [Fact]
        public void TestInsertedEntitiesIdentities_OutputIdentity()
        {
            _context.BulkInsert(_collection, InsertOptions.OutputIdentity);
            Assert.True(_collection.All(model => model.Id != 0));
        }

        [Fact]
        public void TestInsertValuesSavedCorrectly_OutputIdentity()
        {
            _context.BulkInsert(_collection, InsertOptions.OutputIdentity);
            var simpleModels = _context.SimpleModel
                .OrderBy(model => model.Id)
                .ToList();

            Assert.Equal(simpleModels.Count, _collection.Count);

            for (var i = 0; i < _collection.Count; i++)
            {
                var entity = _collection[i];
                var saved = simpleModels[i];
                Assert.Equal(entity.Id, saved.Id);
                Assert.Equal(entity.DateTime, saved.DateTime);
                Assert.Equal(entity.DoubleValue, saved.DoubleValue);
                Assert.Equal(entity.IntValue, saved.IntValue);
                Assert.Equal(entity.Type, saved.Type);
                Assert.Equal(entity.StringProperty, saved.StringProperty);
                Assert.Equal(entity.LatitudeLongitude.SpatialEquals(saved.LatitudeLongitude), true);
            }
        }

        #endregion

        #region Output Computed Set

        [Fact]
        public void TestAffectedRowsCount_OutputComputed()
        {
            var rowsCount = _context.BulkInsert(_collection, InsertOptions.OutputComputed);
            Assert.Equal(rowsCount, _collection.Count);
        }

        [Fact]
        public void TestInsertedEntitiesIdentities_OutputComputed()
        {
            _context.BulkInsert(_collection, InsertOptions.OutputComputed);
            Assert.True(_collection.All(model => model.Id == 0));
        }

        [Fact]
        public void TestInsertValuesSavedCorrectly_OutputComputed()
        {
            _context.BulkInsert(_collection, InsertOptions.OutputComputed);
            var simpleModels = _context.SimpleModel
                .OrderBy(model => model.Id)
                .ToList();

            Assert.Equal(simpleModels.Count, _collection.Count);

            for (var i = 0; i < _collection.Count; i++)
            {
                var entity = _collection[i];
                var saved = simpleModels[i];
                Assert.NotEqual(entity.Id, saved.Id);
                Assert.Equal(entity.DateTime, saved.DateTime);
                Assert.Equal(entity.DoubleValue, saved.DoubleValue);
                Assert.Equal(entity.IntValue, saved.IntValue);
                Assert.Equal(entity.Type, saved.Type);
                Assert.Equal(entity.StringProperty, saved.StringProperty);
                Assert.Equal(entity.LatitudeLongitude.SpatialEquals(saved.LatitudeLongitude), true);
            }
        }

        #endregion
        
        #region Output Indentity & Computed Set

        [Fact]
        public void TestAffectedRowsCount_OutputIdentityComputed()
        {
            var rowsCount = _context.BulkInsert(_collection, InsertOptions.OutputIdentity | InsertOptions.OutputComputed);
            Assert.Equal(rowsCount, _collection.Count);
        }

        [Fact]
        public void TestInsertedEntitiesIdentities_OutputIdentityComputed()
        {
            _context.BulkInsert(_collection, InsertOptions.OutputIdentity | InsertOptions.OutputComputed);
            Assert.True(_collection.All(model => model.Id != 0));
        }

        [Fact]
        public void TestInsertValuesSavedCorrectly_OutputIdentityComputed()
        {
            _context.BulkInsert(_collection, InsertOptions.OutputIdentity | InsertOptions.OutputComputed);
            var simpleModels = _context.SimpleModel
                .OrderBy(model => model.Id)
                .ToList();

            Assert.Equal(simpleModels.Count, _collection.Count);

            for (var i = 0; i < _collection.Count; i++)
            {
                var entity = _collection[i];
                var saved = simpleModels[i];
                Assert.Equal(entity.Id, saved.Id);
                Assert.Equal(entity.DateTime, saved.DateTime);
                Assert.Equal(entity.DoubleValue, saved.DoubleValue);
                Assert.Equal(entity.IntValue, saved.IntValue);
                Assert.Equal(entity.Type, saved.Type);
                Assert.Equal(entity.StringProperty, saved.StringProperty);
                Assert.Equal(entity.LatitudeLongitude.SpatialEquals(saved.LatitudeLongitude), true);
            }
        }

        #endregion

        public void Dispose()
        {
            ClearTable();
        }

        private void ClearTable()
        {
            _context.SimpleModel.RemoveRange(_context.SimpleModel.ToList());
            _context.SaveChanges();
        }
    }
}