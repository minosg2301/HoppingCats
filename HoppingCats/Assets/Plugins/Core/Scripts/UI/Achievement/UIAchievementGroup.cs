using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public class UIAchievementGroup : MonoBehaviour
    {
        public AchievementGroupId groupId;

        public List<UIAchievement> UIAchievements { get; private set; }

        private void Awake()
        {
            UIAchievements = GetComponentsInChildren<UIAchievement>().ToList();

            var achievements = UserAchievement.Ins.FindByGroup(groupId);
            achievements.ForEach((achievement, i) =>
            {
                if(UIAchievements.Count <= i)
                {
                    UIAchievements.Add(Instantiate(UIAchievements[0], transform));
                }

                UIAchievements[i].SetData(achievement);
            });

            // remove unused ui shop item
            for(int i = achievements.Count; i < UIAchievements.Count; i++)
            {
                Destroy(UIAchievements[i].gameObject);
            }
        }

        private void OnValidate()
        {
            var group = AchievementAsset.Ins.FindGroup(groupId);
            if(group)
            {
                gameObject.name = group.name + " Group";
            }
        }
    }
}