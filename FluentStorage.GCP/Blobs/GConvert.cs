﻿using System;
using System.Collections.Generic;
using System.Text;
using Objects = Google.Apis.Storage.v1.Data.Objects;
using Object = Google.Apis.Storage.v1.Data.Object;
using FluentStorage.Blobs;

using Google.Api.Gax;
using System.Threading.Tasks;
using System.Collections;
using FluentStorage.Utils.Extensions;
using Google.Apis.Storage.v1.Data;

namespace FluentStorage.Gcp.CloudStorage.Blobs {
	static class GConvert {
		public static Blob ToBlob(Object go) {
			var blob = new Blob(go.Name) {
				LastModificationTime = go.Updated,
				MD5 = go.Md5Hash.Base64DecodeAsBytes().ToHexString(),
				Size = (long?)go.Size,
			};

			if (go.Metadata?.Count > 0)
				blob.Metadata.AddRange(go.Metadata);

			blob.TryAddProperties(
			   "ContentType", go.ContentType,
			   "CacheControl", go.CacheControl,
			   "ComponentControl", go.ComponentCount.ToString(),
			   "ContentDisposition", go.ContentDisposition,
			   "ContentEncoding", go.ContentEncoding,
			   "ContentLanguage", go.ContentLanguage,
			   "ContentType", go.ContentType,
			   "Crc32", go.Crc32c,
			   "ETag", go.ETag,
			   "EventBaseHold", go.EventBasedHold.ToString(),
			   "Generation", go.Generation.ToString(),
			   "Id", go.Id,
			   "KmsKeyName", go.KmsKeyName,
			   "MediaLink", go.MediaLink,
			   "Metageneration", go.Metageneration,
			   "Owner", go.Owner,
			   "RetentionExpirationTime", go.RetentionExpirationTime,
			   "StorageClass", go.StorageClass,
			   "TemporaryHold", go.TemporaryHold,
			   "TimeCreated", go.TimeCreated,
			   "TimeDeleted", go.TimeDeleted,
			   "TimeStorageClassUpdated", go.TimeStorageClassUpdated);

			return blob;
		}

		public static IEnumerable<Blob> ToBlobs(IEnumerable<Object> objects, ListOptions options) {
			foreach (Object obj in objects) {
				Blob item = ToBlob(obj);

				if (options.FilePrefix != null && !item.Name.StartsWith(options.FilePrefix))
					continue;

				if (options.BrowseFilter != null && !options.BrowseFilter(item))
					continue;

				yield return item;
			}

			yield break;
		}

		public static async Task<IReadOnlyCollection<Blob>> ToBlobsAsync(PagedAsyncEnumerable<Objects, Object> objects, ListOptions options) {
			var result = new List<Blob>();

			await foreach (Object obj in objects) {

				Blob blob = ToBlob(obj);

				if (options.FilePrefix != null && !blob.Name.StartsWith(options.FilePrefix))
					continue;

				if (options.BrowseFilter != null && !options.BrowseFilter(blob))
					continue;

				result.Add(blob);

				if (options.MaxResults != null && result.Count >= options.MaxResults.Value)
					break;
			}

			return result;
		}

	}
}