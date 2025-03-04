﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NGitLab.Mock.Clients
{
    internal sealed class ProjectBadgeClient : ClientBase, IProjectBadgeClient
    {
        private readonly int _projectId;

        public ProjectBadgeClient(ClientContext context, int projectId)
            : base(context)
        {
            _projectId = projectId;
        }

        public Models.Badge this[int id]
        {
            get
            {
                using (Context.BeginOperationScope())
                {
                    var project = GetProject(_projectId, ProjectPermission.View);
                    var badge = project.Badges.GetById(id);
                    if (badge == null)
                        throw new GitLabNotFoundException($"Badge with id '{id}' does not exist in project with id '{_projectId}'");

                    return badge.ToBadgeModel();
                }
            }
        }

        public IEnumerable<Models.Badge> All
        {
            get
            {
                using (Context.BeginOperationScope())
                {
                    var project = GetProject(_projectId, ProjectPermission.View);
                    var groupBadges = (project.Group != null) ? GetGroup(project.Group.Id, GroupPermission.View).Badges.Select(badge => badge.ToBadgeModel()) : Array.Empty<Models.Badge>();
                    return groupBadges.Union(project.Badges.Select(badge => badge.ToBadgeModel())).ToList();
                }
            }
        }

        public IEnumerable<Models.Badge> ProjectsOnly
        {
            get
            {
                using (Context.BeginOperationScope())
                {
                    return All.Where(badge => badge.Kind == Models.BadgeKind.Project).ToList();
                }
            }
        }

        public Models.Badge Create(Models.BadgeCreate badge)
        {
            EnsureUserIsAuthenticated();

            using (Context.BeginOperationScope())
            {
                var createdBadge = GetProject(_projectId, ProjectPermission.Edit).Badges.Add(badge.LinkUrl, badge.ImageUrl);
                return createdBadge.ToBadgeModel();
            }
        }

        public void Delete(int id)
        {
            EnsureUserIsAuthenticated();

            using (Context.BeginOperationScope())
            {
                var badgeToRemove = GetProject(_projectId, ProjectPermission.View).Badges.FirstOrDefault(b => b.Id == id);
                if (badgeToRemove == null)
                {
                    throw new GitLabNotFoundException($"Badge with id '{id}' does not exist in project with id '{_projectId}'");
                }

                GetProject(_projectId, ProjectPermission.Edit).Badges.Remove(badgeToRemove);
            }
        }

        public Models.Badge Update(int id, Models.BadgeUpdate badge)
        {
            using (Context.BeginOperationScope())
            {
                var badgeToUpdate = GetProject(_projectId, ProjectPermission.Edit).Badges.FirstOrDefault(b => b.Id == id);
                if (badgeToUpdate == null)
                {
                    throw new GitLabNotFoundException($"Badge with id '{id}' does not exist in project with id '{_projectId}'");
                }

                badgeToUpdate.LinkUrl = badge.LinkUrl;
                badgeToUpdate.ImageUrl = badge.ImageUrl;
                return badgeToUpdate.ToBadgeModel();
            }
        }
    }
}
