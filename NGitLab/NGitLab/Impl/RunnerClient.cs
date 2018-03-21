﻿using System.Collections.Generic;
using NGitLab.Models;

namespace NGitLab.Impl
{
    public class RunnerClient : IRunnerClient
    {
        private readonly API _api;

        public RunnerClient(API api)
        {
            _api = api;
        }

        public IEnumerable<Runner> Accessible => _api.Get().GetAll<Runner>(Runner.Url);

        public IEnumerable<Runner> All => _api.Get().GetAll<Runner>(Runner.Url + "/all");

        public Runner this[int id] => _api.Get().To<Runner>(Runner.Url + "/" + id);

        public IEnumerable<Runner> GetAllRunnersWithScope(RunnerScope scope)
        {
            string url = Runner.Url + "/all";
            url = Utils.AddParameter(url, "scope", scope.ToString().ToLowerInvariant());
            return _api.Get().GetAll<Runner>(url);
        }

        public IEnumerable<Runner> OfProject(int projectId)
        {
            return _api.Get().GetAll<Runner>(Project.Url + $"/{projectId}" + Runner.Url);
        }

        public void Delete(Runner runner) => Delete(runner.Id);

        public void Delete(int runnerId)
        {
            _api.Delete().Execute(Runner.Url + "/" + runnerId);
        }

        public Runner Update(int runnerId, RunnerUpdate runnerUpdate)
        {
            string url = $"{Runner.Url}/{runnerId}";
            return _api.Put().With(runnerUpdate).To<Runner>(url);
        }

        public IEnumerable<Job> GetJobs(int runnerId, JobScope scope)
        {
            var url = $"{Runner.Url}/{runnerId}/jobs";

            if (scope != JobScope.All)
            {
                url = Utils.AddParameter(url, "status", scope.ToString().ToLowerInvariant());
            }

            return _api.Get().GetAll<Job>(url);
        }

        IEnumerable<Runner> IRunnerClient.GetAvailableRunners(int projectId)
        {
            string url = $"{Project.Url}/{projectId}/runners";
            return _api.Get().GetAll<Runner>(url);
        }

        public Runner EnableRunner(int projectId, RunnerId runnerId)
        {
            string url = $"{Project.Url}/{projectId}/runners";
            return _api.Post().With(runnerId).To<Runner>(url);
        }

        public void DisableRunner(int projectId, RunnerId runnerId)
        {
            string url = $"{Project.Url}/{projectId}/runners/{runnerId.Id}";
            _api.Delete().Execute(url);
        }
    }
}