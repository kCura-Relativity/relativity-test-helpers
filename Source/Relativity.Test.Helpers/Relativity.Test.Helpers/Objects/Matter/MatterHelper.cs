﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.Services.Matter;
using Relativity.API;
using kCura.Relativity.Client;
using Relativity.Test.Helpers.Exceptions;

namespace Relativity.Test.Helpers.Objects.Matter
{
	public class MatterHelper
	{
		private TestHelper _helper;

		public MatterHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public int Create(string matterName, int clientArtifactID)
		{
			int matterArtifactID;

			using (var matterManager = _helper.GetServicesManager().CreateProxy<IMatterManager>(ExecutionIdentity.CurrentUser))
			{
				var matterDTO = new Services.Matter.Matter
				{
					Name = matterName,
					Client = new Relativity.Services.Client.ClientRef(clientArtifactID),
					Number = new Random().Next(1000).ToString(),
					Status = new Relativity.Services.Choice.ChoiceRef(671),
					Notes = "Integration Test Matter"
				};

				matterArtifactID = matterManager.CreateSingleAsync(matterDTO).Result;
			}
			return matterArtifactID;
		}

		public int QueryMatterByName(string name)
		{
			int matterID;
			MatterQueryResultSet results;

			var query = new Services.Query
			{
				Condition = $"('Name' == '{name}')"
			};

			using (var matterManager = _helper.GetServicesManager().CreateProxy<IMatterManager>(ExecutionIdentity.CurrentUser))
			{
				results = matterManager.QueryAsync(query).Result;
			}

			if (results.Success)
			{
				matterID = results.Results[0].Artifact.ArtifactID;
			}
			else
			{
				throw new IntegrationTestException($"Failed to retrieve matter by name equal to {name}");
			}

			return matterID;
		}

		public void Delete(int matterID)
		{
			using (var matterManager = _helper.GetServicesManager().CreateProxy<IMatterManager>(ExecutionIdentity.CurrentUser))
			{
				matterManager.DeleteSingleAsync(matterID).Wait();
			}
		}

	}
}
