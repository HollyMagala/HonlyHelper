using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.HonlyHelper
{
    static class StaticExtensions
    {
        public struct listMoment
        {
            public Entity entity;
            public float distance;
        }

        //NOTE: number of returns includes self
        static public List<Entity> GetNearestN<T>(this Tracker tracker, Vector2 nearestTo, int count) where T : Entity
        {
            
            listMoment[] entityList = new listMoment[count];
            listMoment holdingBox;
            listMoment holdingBox2;
            foreach (T entity in tracker.GetEntities<T>())
            {
                float distance = Vector2.DistanceSquared(nearestTo, entity.Position);

                //check if array has empty slots, if any slots are empty, [0] is empty
                if (entityList[0].entity == null)
                {
                    holdingBox2.entity = entity;
                    holdingBox2.distance = distance;

                    bool putInList = false;
                    //check for empty slots, start from top
                    for (int i = count; i > 0; i--)
                    {
                        if (entityList[i].entity != null)
                        {
                            entityList[i].entity = entity;
                            entityList[i].distance = distance;
                            putInList = true;
                            break;
                        }
                        else if (entityList[i].distance > holdingBox2.distance)
                        {
                            holdingBox = entityList[i];
                            entityList[i] = holdingBox2;
                            holdingBox2 = holdingBox;
                        }
                    }
                    if (!putInList)
                    {
                        entityList[0].entity = entity;
                        entityList[0].distance = distance;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (distance < entityList[i].distance)
                        {
                            holdingBox = entityList[i];
                            entityList[i].entity = entity;
                            entityList[i].distance = distance;
                            if (i != 0)
                            {
                                entityList[i - 1] = holdingBox;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            //extract the entities and make a list
            List<Entity> returnList = new List<Entity>();
            foreach (listMoment duo in entityList)
            {
                returnList.Add(duo.entity);
            }
            return returnList;
        }

        static public List<T> GetWithinRadius<T>(this Tracker tracker, Vector2 nearestTo, float radius) where T : Entity
        {
            List<T> entityList = new List<T>();
            float radiusSquared = (float)Math.Pow(radius, 2);

            foreach (T entity in tracker.GetEntities<T>())
            {
                if(Vector2.DistanceSquared(nearestTo, entity.Position) < radiusSquared)
                {
                    entityList.Add(entity);
                }
            }
                return entityList;
        }

    }
}
