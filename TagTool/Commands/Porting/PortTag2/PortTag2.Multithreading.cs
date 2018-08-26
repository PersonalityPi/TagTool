using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TagTool.Audio;
using TagTool.Cache;
using TagTool.Common;
using TagTool.Damage;
using TagTool.Geometry;
using TagTool.Havok;
using TagTool.Serialization;
using TagTool.Tags;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Porting
{
    public partial class PortTag2Command : Command
    {
        public class ConvertTagTask : Task<CachedTagInstance>
        {
            #region Constructors


            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<CachedTagInstance> function) : base(function)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<CachedTagInstance> function, CancellationToken cancellationToken) : base(function, cancellationToken)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<CachedTagInstance> function, TaskCreationOptions creationOptions) : base(function, creationOptions)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<CachedTagInstance> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, cancellationToken, creationOptions)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, CachedTagInstance> function, object state) : base(function, state)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, CachedTagInstance> function, object state, CancellationToken cancellationToken) : base(function, state, cancellationToken)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, CachedTagInstance> function, object state, TaskCreationOptions creationOptions) : base(function, state, creationOptions)
            {
            }

            public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, CachedTagInstance> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, state, cancellationToken, creationOptions)
            {
            }

            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object> function) : base(function) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object> function, CancellationToken cancellationToken) : base(function, cancellationToken) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object> function, TaskCreationOptions creationOptions) : base(function, creationOptions) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, object> function, object state) : base(function, state) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, cancellationToken, creationOptions) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, object> function, object state, CancellationToken cancellationToken) : base(function, state, cancellationToken) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, object> function, object state, TaskCreationOptions creationOptions) : base(function, state, creationOptions) { BlamTag = blamTag; }
            //public ConvertTagTask(CacheFile.IndexItem blamTag, Func<object, object> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, state, cancellationToken, creationOptions) { BlamTag = blamTag; }

            #endregion

            private CacheFile.IndexItem BlamTag = null;

            public struct ConvertTagTask_AssignmentOperation
            {
                public FieldInfo FieldInfo;
                public object Parent;
            }
            private ConcurrentBag<ConvertTagTask_AssignmentOperation> AssignmentOperations = new ConcurrentBag<ConvertTagTask_AssignmentOperation>();


            public List<ConvertTagTask_AssignmentOperation> GetAssignmentOperations()
            {
                return AssignmentOperations.ToList();
            }
            public void AddAssignmentOperations(object parent, FieldInfo fieldinfo)
            {
                AssignmentOperations.Add(new ConvertTagTask_AssignmentOperation { Parent = parent, FieldInfo = fieldinfo });
            }

            public override string ToString()
            {
                return $"({this.Status.ToString()}) {BlamTag.ToString()}";
            }
        }

        private Mutex TagJobsMutex = new Mutex();
        private Dictionary<int, ConvertTagTask> ConvertTagTasks = new Dictionary<int, ConvertTagTask>();

        ConvertTagTask CreateConvertTagJob(
            Stream cacheStream,
            Dictionary<TagTool.Common.ResourceLocation, Stream> resourceStreams,
            CacheFile.IndexItem blamTag,
            object parent, FieldInfo
            fieldinfo)
        {
            int index = blamTag.Index;

            TagJobsMutex.WaitOne();

            ConvertTagTask task = null;
            if (ConvertTagTasks.ContainsKey(index))
            {
                task = ConvertTagTasks[index];
                task.AddAssignmentOperations(parent, fieldinfo);
            }
            else
            {
                task = new ConvertTagTask(blamTag, () =>
                {
                    return ConvertTagSync(/*cacheStream,*/ resourceStreams, blamTag);
                });
                task.AddAssignmentOperations(parent, fieldinfo);
                task.Start();

                ConvertTagTasks[index] = task;
            }

            TagJobsMutex.ReleaseMutex();

            // When the task is completed, we'll assign the result of the tasks to all of parents and their fields

            return task;
        }
    }
}