using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common.Contract;
using System.Linq;

namespace Emotion.Shared
{
    public class Core
    {
       
        private static async Task<Microsoft.ProjectOxford.Common.Contract.Emotion[]> GetHappiness(Stream stream)
        {
            string emotionKey = "1f51a38d09fe48318f2faf504f70d8e4";
            EmotionServiceClient emotionClient = new EmotionServiceClient(emotionKey);
            var emotionResults = await emotionClient.RecognizeAsync(stream);
            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }
            return emotionResults;
        }
        public static async Task<float> GetAverageHappinessScore(Stream stream)
        {
            Microsoft.ProjectOxford.Common.Contract.Emotion[] emotionResults = await GetHappiness(stream);
            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }
            return score / emotionResults.Count();
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            double result = Math.Round(score, 2);
            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }
}
