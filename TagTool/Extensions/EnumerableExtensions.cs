using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.Tags;

namespace System.Linq
{
	static class EnumerableExtensions
	{
		public static TagBlock<TSource> ToBlock<TSource>(this IEnumerable<TSource> source) where TSource : TagStructure
			=> new TagBlock<TSource>(source);
	}
}
