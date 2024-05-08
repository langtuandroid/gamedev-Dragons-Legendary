using UnityEngine;

public static class SoundControllerHelper
{
	public static SoundSubItem[] ChooseSubSounds(SoundItem audioItem, SoundObject useExistingAudioObj)
	{
		return ChooseSubSounds(audioItem, audioItem.PickMode, useExistingAudioObj);
	}

	private static SoundSubItem ChooseSingleSubSound(SoundItem audioItem, SoundPickSubItemMode pickMode, SoundObject useExistingAudioObj)
	{
		return ChooseSubSounds(audioItem, pickMode, useExistingAudioObj)[0];
	}

	private static SoundSubItem[] ChooseSubSounds(SoundItem audioItem, SoundPickSubItemMode pickMode, SoundObject useExistingAudioObj)
	{
		if (audioItem._subItemsList == null)
		{
			return null;
		}
		int num = audioItem._subItemsList.Length;
		if (num == 0)
		{
			return null;
		}
		int num2 = 0;
		bool flag = !object.ReferenceEquals(useExistingAudioObj, null);
		int num3 = (!flag) ? audioItem._lastChosen : useExistingAudioObj._lastSubIndex;
		if (num > 1)
		{
			switch (pickMode)
			{
			case SoundPickSubItemMode.Disabled:
				return null;
			case SoundPickSubItemMode.StartLoopSequenceWithFirst:
				num2 = (flag ? ((num3 + 1) % num) : 0);
				break;
			case SoundPickSubItemMode.Sequence:
				num2 = (num3 + 1) % num;
				break;
			case SoundPickSubItemMode.SequenceWithRandomStart:
				num2 = ((num3 != -1) ? ((num3 + 1) % num) : Random.Range(0, num));
				break;
			case SoundPickSubItemMode.Random:
				num2 = RandomizeItems(audioItem, allowSameElementTwiceInRow: true, num3);
				break;
			case SoundPickSubItemMode.RandomNotSameTwice:
				num2 = RandomizeItems(audioItem, allowSameElementTwiceInRow: false, num3);
				break;
			case SoundPickSubItemMode.AllSimultaneously:
			{
				SoundSubItem[] array = new SoundSubItem[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = audioItem._subItemsList[i];
				}
				return array;
			}
			case SoundPickSubItemMode.TwoSimultaneously:
				return new SoundSubItem[2]
				{
					ChooseSingleSubSound(audioItem, SoundPickSubItemMode.RandomNotSameTwice, useExistingAudioObj),
					ChooseSingleSubSound(audioItem, SoundPickSubItemMode.RandomNotSameTwice, useExistingAudioObj)
				};
			case SoundPickSubItemMode.RandomNotSameTwiceOddsEvens:
				num2 = RandomizeItems(audioItem, allowSameElementTwiceInRow: false, num3, switchOddsEvens: true);
				break;
			}
		}
		if (flag)
		{
			useExistingAudioObj._lastSubIndex = num2;
		}
		else
		{
			audioItem._lastChosen = num2;
		}
		return new SoundSubItem[1]
		{
			audioItem._subItemsList[num2]
		};
	}

	private static int RandomizeItems(SoundItem audioItem, bool allowSameElementTwiceInRow, int lastChosen, bool switchOddsEvens = false)
	{
		int num = audioItem._subItemsList.Length;
		int result = 0;
		float num2 = 0f;
		float max;
		if (!allowSameElementTwiceInRow)
		{
			if (lastChosen >= 0)
			{
				num2 = audioItem._subItemsList[lastChosen].SummedProbability;
				if (lastChosen >= 1)
				{
					num2 -= audioItem._subItemsList[lastChosen - 1].SummedProbability;
				}
			}
			else
			{
				num2 = 0f;
			}
			max = 1f - num2;
		}
		else
		{
			max = 1f;
		}
		float num3 = Random.Range(0f, max);
		int i;
		for (i = 0; i < num - 1; i++)
		{
			float num4 = audioItem._subItemsList[i].SummedProbability;
			if (switchOddsEvens && CheckOdd(i) == CheckOdd(lastChosen))
			{
				continue;
			}
			if (!allowSameElementTwiceInRow)
			{
				if (i == lastChosen && (num4 != 1f || !audioItem._subItemsList[i]._disableOtherSubitems))
				{
					continue;
				}
				if (i > lastChosen)
				{
					num4 -= num2;
				}
			}
			if (num4 > num3)
			{
				result = i;
				break;
			}
		}
		if (i == num - 1)
		{
			result = num - 1;
		}
		return result;
	}

	private static bool CheckOdd(int i)
	{
		return i % 2 != 0;
	}
}
