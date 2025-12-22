using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Money", "FireRateLevel", "DamageLevel", "BulletSizeLevel", "BulletSpeedLevel", "lastWaveIndex", "SfxVolume", "BackgroundVolume")]
	public class ES3UserType_PlayerSaveData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerSaveData() : base(typeof(JollyPanda.LastFlag.Database.PlayerSaveData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (JollyPanda.LastFlag.Database.PlayerSaveData)obj;
			
			writer.WriteProperty("Money", instance.Money, ES3Type_int.Instance);
			writer.WriteProperty("FireRateLevel", instance.FireRateLevel, ES3Type_int.Instance);
			writer.WriteProperty("DamageLevel", instance.DamageLevel, ES3Type_int.Instance);
			writer.WriteProperty("BulletSizeLevel", instance.BulletSizeLevel, ES3Type_int.Instance);
			writer.WriteProperty("BulletSpeedLevel", instance.BulletSpeedLevel, ES3Type_int.Instance);
			writer.WriteProperty("lastWaveIndex", instance.lastWaveIndex, ES3Type_int.Instance);
			writer.WriteProperty("SfxVolume", instance.SfxVolume, ES3Type_float.Instance);
			writer.WriteProperty("BackgroundVolume", instance.BackgroundVolume, ES3Type_float.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (JollyPanda.LastFlag.Database.PlayerSaveData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Money":
						instance.Money = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "FireRateLevel":
						instance.FireRateLevel = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "DamageLevel":
						instance.DamageLevel = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "BulletSizeLevel":
						instance.BulletSizeLevel = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "BulletSpeedLevel":
						instance.BulletSpeedLevel = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "lastWaveIndex":
						instance.lastWaveIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "SfxVolume":
						instance.SfxVolume = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "BackgroundVolume":
						instance.BackgroundVolume = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new JollyPanda.LastFlag.Database.PlayerSaveData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_PlayerSaveDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerSaveDataArray() : base(typeof(JollyPanda.LastFlag.Database.PlayerSaveData[]), ES3UserType_PlayerSaveData.Instance)
		{
			Instance = this;
		}
	}
}