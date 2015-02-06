﻿using System;
using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class RangessAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggRangesBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new RangeBucketAggregation("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter>
					{
						new ToRangeAggregationParameter(1.5),
						new ToFromRangeAggregationParameter(1.5,2.0),
						new FromRangeAggregationParameter(2.0)
					})
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("2.0", aggResult.Buckets[2].FromAsString);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationKeyedWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new RangeBucketAggregation("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter>
					{
						new ToRangeAggregationParameter(1.5),
						new ToFromRangeAggregationParameter(1.5,2.0),
						new FromRangeAggregationParameter(2.0)
					})
					{
						Keyed= true
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
				var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("2.0-*");

				Assert.AreEqual("2.0", test.FromAsString);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationWithRangeKeysWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new RangeBucketAggregation("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter>
					{
						new ToRangeAggregationParameter(1.5)
						{
							Key = "one"
						},
						new ToFromRangeAggregationParameter(1.5,2.0)
						{
							Key = "two"
						},
						new FromRangeAggregationParameter(2.0)
						{
							Key = "three"
						}
					})
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("three", aggResult.Buckets[2].Key);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationKeyedWithRangeKeysWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new RangeBucketAggregation("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter>
					{
						new ToRangeAggregationParameter(1.5)
						{
							Key = "one"
						},
						new ToFromRangeAggregationParameter(1.5,2.0)
						{
							Key = "two"
						},
						new FromRangeAggregationParameter(2.0)
						{
							Key = "three"
						}
					})
					{
						Keyed= true
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
				var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("three");

				Assert.AreEqual("2.0", test.FromAsString);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationWithRangeKeysWithMaxMetricSubAggWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new RangeBucketAggregation("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter>
					{
						new ToRangeAggregationParameter(1.5)
						{
							Key = "one"
						},
						new ToFromRangeAggregationParameter(1.5,2.0)
						{
							Key = "two"
						},
						new FromRangeAggregationParameter(2.0)
						{
							Key = "three"
						}
					})
					{
						Aggs = new List<IAggs>
						{
							new MaxMetricAggregation("maxi", "lift")
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				var max = aggResult.Buckets[2].GetSingleMetricSubAggregationValue<double>("maxi");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("three", aggResult.Buckets[2].Key);
				Assert.AreEqual(2.9, max);
			}
		}

		[Test]
		public void SearchAggDateRangesBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new DateRangeBucketAggregation("testRangesBucketAggregation", "dateofdetails", "MM-yyy", new List<RangeAggregationParameter>
					{
						new ToRangeAggregationParameter("now-10M/M"),
						//new ToFromRangeAggregationParameter("now-10M/M", "now-10M/M"),
						new FromRangeAggregationParameter("now-10M/M")
					})
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("2.0", aggResult.Buckets[2].FromAsString);
			}
		}

	}
}