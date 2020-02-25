﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Test.Helpers.WorkspaceHelpers;
using Field = kCura.Relativity.Client.Field;
using FieldType = Relativity.Services.Interfaces.Field.Models.FieldType;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class FieldsHelperIntegrationTests
	{
		private IHelper _testHelper;
		private IServicesMgr _servicesManager;
		private IRSAPIClient _rsapiClient;
		private IDBContext _dbContext;

		private int _workspaceId;
		private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";

		private const string _testFieldName = "Production::Sort Order";
		private int _productionSortOrderFieldArtifactId;
		private const int _productionSortOrderFieldCount = 1;

		private KeplerHelper _keplerHelper;
		private bool useDbContext;

		[OneTimeSetUp]
		public void SetUp()
		{
			//ARRANGE
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			_testHelper = new TestHelper(configDictionary);
			_servicesManager = _testHelper.GetServicesManager();
			_rsapiClient = _testHelper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Create workspace
			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			_rsapiClient.APIOptions.WorkspaceID = _workspaceId;

			//Query for field ID to be used in test
			var fieldNameCondition = new TextCondition("Name", TextConditionEnum.EqualTo, _testFieldName);
			var query = new Query<RDO> { ArtifactTypeID = 14, Condition = fieldNameCondition };
			query.Fields.Add(new FieldValue("Artifact ID"));
			query.Fields.Add(new FieldValue("Name"));

			//Query and throw exception if the query fails
			var results = _rsapiClient.Repositories.RDO.Query(query);
			if (!results.Success) throw new Exception("Failed to query for field.");
			_productionSortOrderFieldArtifactId = results.Results.First().Artifact.ArtifactID;

			_keplerHelper = new KeplerHelper();
			bool isKeplerCompatible = _keplerHelper.IsVersionKeplerCompatibleAsync().Result;
			useDbContext = !isKeplerCompatible || ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
			if (useDbContext)
			{
				_dbContext = _testHelper.GetDBContext(_workspaceId);
			}
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			//Delete workspace
			DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_servicesManager = null;
			_rsapiClient = null;
			_dbContext = null;
			_testHelper = null;
		}

		[Test]
		public void GetFieldArtifactIdTest()
		{
			//ACT
			int fieldArtifactId;
			if (useDbContext)
			{
				fieldArtifactId = Fields.GetFieldArtifactID(_testFieldName, _dbContext);
			}
			else
			{
				fieldArtifactId = Fields.GetFieldArtifactID(_testFieldName, _workspaceId, _keplerHelper);
			}

			//ASSERT
			Assert.AreEqual(fieldArtifactId, _productionSortOrderFieldArtifactId);
		}

		[Test]
		public void GetFieldArtifactIdTest_Invalid()
		{
			//ACT
			var invalidFieldName = "";

			//ASSERT
			if (useDbContext)
			{
				Assert.Throws<ArgumentNullException>(() => Fields.GetFieldArtifactID(invalidFieldName, _dbContext));
			}
			else
			{
				int fieldArtifactId = Fields.GetFieldArtifactID(invalidFieldName, _workspaceId, _keplerHelper);
				Assert.AreEqual(0, fieldArtifactId);
			}
		}

		[Test]
		public void GetFieldCountTest()
		{
			//ACT
			int fieldCount;
			if (useDbContext)
			{
				fieldCount = Fields.GetFieldCount(_dbContext, _productionSortOrderFieldArtifactId);
			}
			else
			{
				fieldCount = Fields.GetFieldCount(_productionSortOrderFieldArtifactId, _workspaceId, _keplerHelper);
			}

			//ASSERT
			Assert.AreEqual(fieldCount, _productionSortOrderFieldCount);
		}

		[Test]
		public void GetFieldCountTest_Invalid()
		{
			//ACT
			var invalidfieldArtifactId = -1;

			//ASSERT
			if (useDbContext)
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => Fields.GetFieldCount(_dbContext, invalidfieldArtifactId));
			}
			else
			{
				int fieldCount = Fields.GetFieldCount(invalidfieldArtifactId, _workspaceId, _keplerHelper);
				Assert.AreEqual(0, fieldCount);
			}
		}

		[Test]
		public void CreateFieldDateTest()
		{
			//Act
			var fieldId = Fields.CreateField_Date(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.Date);
		}
		[Test]
		public void CreateFieldUserTest()
		{
			//Act
			var fieldId = Fields.CreateField_User(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.User);

		}
		[Test]
		public void CreateFieldFixedLengthTextTest()
		{
			//Act
			var fieldId = Fields.CreateField_FixedLengthText(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.FixedLength);

		}
		[Test]
		public void CreateFieldLongTextTest()
		{
			//Act
			var fieldId = Fields.CreateField_LongText(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.LongText);

		}
		[Test]
		public void CreateFieldWholeNumberTest()
		{
			//Act
			var fieldId = Fields.CreateField_WholeNumber(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.WholeNumber);

		}
		[Test]
		public void CreateFieldYesNoTest()
		{
			//Act
			var fieldId = Fields.CreateField_YesNO(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.YesNo);

		}
		[Test]
		public void CreateFieldSingleChoiceTest()
		{
			//Act
			var fieldId = Fields.CreateField_SingleChoice(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.SingleChoice);

		}
		[Test]
		public void CreateFieldMultipleChoiceTest()
		{
			//Act
			var fieldId = Fields.CreateField_MultipleChoice(_rsapiClient, _workspaceId);

			//Assert
			var createdFieldType = ReadField(fieldId, _workspaceId);
			Assert.IsTrue(createdFieldType == FieldType.MultipleChoice);

		}

		private FieldType ReadField(int fieldId, int workspaceId)
		{
			using (var fieldManager = _servicesManager.GetProxy<Services.Interfaces.Field.IFieldManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
			{
				var response = fieldManager.ReadAsync(workspaceId, fieldId).Result;
				var fieldType = response.FieldType;
				return fieldType;
			}
		}
	}
}
