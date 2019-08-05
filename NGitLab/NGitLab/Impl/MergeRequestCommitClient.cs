﻿using System.Collections.Generic;
using NGitLab.Models;

namespace NGitLab.Impl
{
    public class MergeRequestCommitClient : IMergeRequestCommitClient
    {
        private readonly API _api;
        private readonly string _commitsPath;

        public MergeRequestCommitClient(API api, string projectPath, int mergeRequestIid)
        {
            _api = api;
            _commitsPath = projectPath + "/merge_requests/" + mergeRequestIid + "/commits?per_page=100";
        }

        public IEnumerable<Commit> All => _api.Get().GetAll<Commit>(_commitsPath);
    }
}