﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	public class DocumentHelperIntegrationTests
	{
		private IHelper testHelper;
		private DocumentHelper SuT;
		private string _workspaceName;
		private IServicesMgr _servicesManager;
		private int _workspaceId;

		[SetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			SuT = new DocumentHelper(new HttpRequestHelper());
			_workspaceName = $"IntTest_{Guid.NewGuid()}";
			_servicesManager = testHelper.GetServicesManager();
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
		}

		[TearDown]
		public void TearDown()
		{
			//Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			testHelper = null;
			SuT = null;
			_servicesManager = null;
		}

		[Test]
		public void GetDocumentIdentifierFieldColumnName()
		{
			// Arrange
			const int fieldArtifactTypeId = 10;
			const string controlNumber = "ControlNumber";

			// Act
			string columnName = SuT.GetDocumentIdentifierFieldColumnName(fieldArtifactTypeId, _workspaceId);

			// Assert
			Assert.AreEqual(controlNumber, columnName);
		}

		[Test]
		public void GetDocumentIdentifierFieldName()
		{
			// Arrange
			const int fieldArtifactTypeId = 10;
			const string controlNumber = "Control Number";

			// Act
			string fieldName = SuT.GetDocumentIdentifierFieldName(fieldArtifactTypeId, _workspaceId);

			// Assert
			Assert.AreEqual(controlNumber, fieldName);
		}
	}
}
