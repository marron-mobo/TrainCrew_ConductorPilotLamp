using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WMPLib;
using TrainCrew;
using System.IO;

namespace 車掌用知らせ灯
{
    public partial class Form1 : Form
    {
        Timer timer;
        StringBuilder sb = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();//レチ知らせ灯
        
        //発車放送
        int IsAvail = 0;
        int IsPlayed = 0;
        string sound;

        public Form1()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;

            //初期化。起動時のみの呼び出しで大丈夫です。
            TrainCrewInput.Init();


            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Tick += circleDraw;
            timer.Interval = 200;
            timer.Start();
            var state = TrainCrewInput.GetTrainState();

            IsAudioFileAvail();
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var state = TrainCrewInput.GetTrainState();

            sb.Clear();
            sb.AppendLine("速度:" + state.Speed.ToString("0.0km/h"));
            sb.AppendLine("戸閉:" + state.AllClose);
            sb.AppendLine(state.nextStaName + " " + state.nextStopType);
            sb.AppendLine("残り" + state.nextStaDistance + "m");
            sb.AppendLine("制限" + state.speedLimit + "km/h");



        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    Button b = (Button)sender;

        //    switch (b.Text)
        //    {
        //        case "EB": TrainCrewInput.SetNotch(-8); break;
        //        case "B6": TrainCrewInput.SetNotch(-7); break;
        //        case "B5": TrainCrewInput.SetNotch(-6); break;
        //        case "B4": TrainCrewInput.SetNotch(-5); break;
        //        case "B3": TrainCrewInput.SetNotch(-4); break;
        //        case "B2": TrainCrewInput.SetNotch(-3); break;
        //        case "B1": TrainCrewInput.SetNotch(-2); break;
        //        case "Holding braking":
        //        case "抑速":
        //            TrainCrewInput.SetNotch(-1);
        //            break;
        //        case "N": TrainCrewInput.SetNotch(0); break;
        //        case "P1": TrainCrewInput.SetNotch(1); break;
        //        case "P2": TrainCrewInput.SetNotch(2); break;
        //        case "P3": TrainCrewInput.SetNotch(3); break;
        //        case "P4": TrainCrewInput.SetNotch(4); break;
        //        case "P5": TrainCrewInput.SetNotch(5); break;

        //        case "Forward": case "前進": TrainCrewInput.SetReverser(1); break;
        //        case "Neutral": case "中立": TrainCrewInput.SetReverser(0); break;
        //        case "Backward": case "後進": TrainCrewInput.SetReverser(-1); break;
        //        case "自動放送": TrainCrewInput.SetButton(InputAction.Housou, true); break;
        //    }
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            TrainCrewInput.Dispose();
        }

        private void circleDraw(object sender, EventArgs e)
        {
            var state = TrainCrewInput.GetTrainState();
            // using System.Drawing;

            // 描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            // ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);

            if (state.AllClose)
            {
                // 緑のブラシを作成する
                Brush brush = new SolidBrush(Color.Green);

                // 円を緑で塗りつぶす
                g.FillEllipse(brush, 10, 20, 70, 70);

                // リソースを解放する
                brush.Dispose();

                //発車放送を流す
                if (IsPlayed == 0)
                {
                    if (IsAvail == 1 && !string.IsNullOrEmpty(sound))
                    {
                        // Windows Media Player を初期化して再生
                        WindowsMediaPlayer player = new WindowsMediaPlayer();
                        player.URL = sound;
                        player.controls.play();
                        IsPlayed = 1;
                    }
                }
            }
            else
            {
                // 線だけの円を描く
                g.DrawEllipse(Pens.Green, 10, 20, 70, 70);
                
                //発車放送リセット
                IsPlayed = 0;
            }

            // リソースを解放する
            g.Dispose();

            // PictureBox1に表示する
            pictureBox1.Image = canvas;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
        }

        public void IsAudioFileAvail()
        {
            try
            {
                // exeファイルと同じディレクトリを取得
                string exePath = Application.StartupPath;
                string soundFilePath = Path.Combine(exePath, "departure.wav");

                // ファイルが存在するか確認
                if (File.Exists(soundFilePath))
                {
                    // ファイルが存在する場合、変数 sound に代入
                    sound = soundFilePath;
                    label1.Text = "発車放送が見つかりました";
                    IsAvail = 1;
                }
                else
                {
                    // ファイルが存在しない場合
                    label1.Text = "発車放送が見つかりません";
                }
            }
            catch (Exception ex)
            {
                // エラーが発生した場合の処理
                label1.Text = "エラー: " + ex.Message;
            }
        }

    }
}
