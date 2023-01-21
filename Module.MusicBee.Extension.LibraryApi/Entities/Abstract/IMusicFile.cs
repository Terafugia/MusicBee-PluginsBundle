﻿using Root.MusicBeeApi;

namespace Module.MusicBee.Extension.LibraryApi.Entities.Abstract;

public interface IMusicFile
{
	string Path { get; }

	string Artist { get; set; }
	string TrackTitle { get; set; }

	string GetTagValue(MetaDataType metaDataType);
	void SetTagValue(MetaDataType metaDataType, string value);

	void Restore();
	void Save();
}