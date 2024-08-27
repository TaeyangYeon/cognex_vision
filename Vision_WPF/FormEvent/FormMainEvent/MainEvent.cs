using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLC;
using Vision;
using UI;
using System.Windows.Controls;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Microsoft.Win32;

namespace FormEvent
{
    public class MainEvent : IMainEvent
    {
        private IPositionChangeEvent positionChangeEvent;
        private ISignalLoop signalLoop;
        private IGrabControl grabControl;
        private IImageFileControl imageFileControl;
        private string currentPosition { get; set; }
        private ListBox currentList { get; set; }
        private TextBlock currentTotalLabel { get; set; }
        private TextBlock currentNGLabel { get; set; }

        public MainEvent(IPositionChangeEvent _positionChangeEvent, ISignalLoop _signalLoop)
        {
            this.positionChangeEvent = _positionChangeEvent;
            this.signalLoop = _signalLoop;
        }

        public MainEvent(IPositionChangeEvent _positionChangeEvent, ISignalLoop _signalLoop, IGrabControl _grabControl, IImageFileControl _imageFileControl)
        {
            this.positionChangeEvent = _positionChangeEvent;
            this.signalLoop = _signalLoop;
            this.grabControl = _grabControl;
            this.imageFileControl = _imageFileControl;
        }

        public void setMainForm(Form_Main form)
        {
            selectPosition("#1", form);

            setCurrentList(form);

            setCurrentLabel(form);
        }

        public void setCurrentPositionAndListBox(Form_Main form, string position)
        {
            currentPosition = position;
            setCurrentList(form);
            setCurrentLabel(form);
        }

        public void selectPosition(string position, Form_Main form)
        {
            currentPosition = position;
            setCurrentList(form);
            setCurrentLabel(form);

            var buttons = new Dictionary<string, Button>
            {
                { "#1", form.position_button1 },
                { "#2", form.position_button2 },
                { "#3", form.position_button3 },
                { "#4", form.position_button4 }
            };

            var lists = new Dictionary<string, ListBox>
            {
                {"#1", form.log_list1},
                {"#2", form.log_list2},
                {"#3", form.log_list3},
                {"#4", form.log_list4}
            };

            positionChangeEvent.changePosition(position, buttons, lists);
        }

        private void setCurrentList(Form_Main form)
        {
            switch (currentPosition)
            {
                case "#1":
                    currentList = form.log_list1;
                    break;
                case "#2":
                    currentList = form.log_list2;
                    break;
                case "#3":
                    currentList = form.log_list3;
                    break;
                case "#4":
                    currentList = form.log_list4;
                    break;
            }
        }

        private void setCurrentLabel(Form_Main form)
        {
            switch (currentPosition)
            {
                case "#1":
                    currentTotalLabel = form.result_total1;
                    currentNGLabel = form.result_ng1;
                    break;
                case "#2":
                    currentTotalLabel = form.result_total2;
                    currentNGLabel = form.result_ng2;
                    break;
                case "#3":
                    currentTotalLabel = form.result_total3;
                    currentNGLabel = form.result_ng3;
                    break;
                case "#4":
                    currentTotalLabel = form.result_total4;
                    currentNGLabel = form.result_ng4;
                    break;
            }
        }

        public async Task socketConnect()
        {
            try
            {
                await Task.Run(() =>
                {
                    signalLoop.connectToServer();
                });
            }
            catch (Exception e)
            {
                throw e;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task<string> getModelName(string address)
        {
            try
            {
                return await signalLoop.getData(PLCCommand.subCommandSub, address);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task testCycle(int index, Form_Main form, int result, (int x, int y) AlignValues)
        {
            // Log_List_Box를 선택된 번호로 설정
            setCurrentPositionAndListBox(form, getCurrentPosition(index));

            // 주소값을 조회
            Dictionary<string, string> address = AddressMap.address(index);

            try
            {
                // Align_Signal을 확인
                int status = int.Parse(await signalLoop.getData(PLCCommand.subCommandSub, address[PLCCommand.subCommandSub]));

                // Align_Signal이 Off이면 종료
                if (status == 0) return;

                // log 저장
                logListInsert("Align Start ON");

                // 결과값에 따라 "OK" / "NG" 를 할당
                string resultCommand = result == 1 ? "OK" : "NG";

                // 결과를 PLC에 전달 및 로그 저장
                signalLoop.writeData(PLCCommand.subCommandBit, address[PLCCommand.subCommandBit], result);
                logListInsert($"Result {resultCommand}");

                // 결과가 OK일 경우 Align_Data를 PLC에 전달
                writeAlign(signalLoop, result, form, (address[PLCCommand.subCommandWord + "x"], address[PLCCommand.subCommandWord + "y"]), AlignValues);

                // Align_Signal이 Off가 되었는지 확인
                signalLoop.checkAlignSignalChanged(status, PLCCommand.subCommandSub, address[PLCCommand.subCommandSub]);

                // 로그 저장
                logListInsert("Align Start Off");

                // 결과값을 Off 상태로 변경
                signalLoop.writeData(PLCCommand.subCommandBit, address[PLCCommand.subCommandBit], 0);

                // 검사 수량 count++
                addTotal();

                // Log_List_Box에 검사별 구분 생성
                logDataDevide();
            }
            catch (NotChangedPlcSignal e)
            {
                // 결과값 및 Align_Data 전달 후 Align_Signal이 Off 되지 않을시
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string getCurrentPosition(int index)
        {
            string position = "";

            switch (index)
            {
                case 1:
                    position = "#1";
                    break;
                case 2:
                    position = "#2";
                    break;
                case 3:
                    position = "#3";
                    break;
                case 4:
                    position = "#4";
                    break;
            }

            return position;
        }

        private void writeAlign(ISignalLoop signalLoop, int result, Form_Main form, (string x, string y) address, (int x, int y) AlignValues)
        {
            if (result == 1)
            {
                int x = AlignValues.x;
                int y = AlignValues.y;

                signalLoop.writeData(PLCCommand.subCommandWord, address.x, x);
                signalLoop.writeData(PLCCommand.subCommandWord, address.y, y);

                logListInsert($"x = {x} | y = {y}");
            }
            else
            {
                addNG();
            }
        }

        private void logListInsert(string value)
        {
            currentList.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss.fff")} _ {value}");
        }

        private void logDataDevide()
        {
            currentList.Items.Insert(0, "");
            currentList.Items.Insert(0, "------------------------------------");
            currentList.Items.Insert(0, "");
        }

        public void addTotal()
        {
            currentTotalLabel.Text = (int.Parse(currentTotalLabel.Text) + 1).ToString();
        }

        public void addNG()
        {
            currentNGLabel.Text = (int.Parse(currentNGLabel.Text) + 1).ToString();
        }

        public async Task socketClose()
        {
            try
            {
                await Task.Run(() =>
                {
                    signalLoop.closeToServer();
                });
            }
            catch (Exception e)
            {
                throw e;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task initializeCamera()
        {
            try
            {
                grabControl.initializeCamera();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void toggleLiveMode(int cameraIndex, bool isLiveMode, CogDisplay display)
        {
            try
            {
                grabControl.toggleLiveMode(isLiveMode);
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
        }

        public async Task<ICogImage> grabImage(int cameraIndex, CogDisplay display)
        {
            return grabControl.grabImage();
        }

        public async Task loadImage(CogDisplay display)
        {
            await imageFileControl.loadImage(getImageFilePath());
        }

        private string getImageFilePath()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select an image file",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            else
            {
                throw new Exception();
            }
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (signalLoop != null)
                    {
                        signalLoop.Dispose();
                        signalLoop = null;
                    }
                    if (grabControl != null)
                    {
                        grabControl.Dispose();
                        grabControl = null;
                    }
                }
                disposed = true;
            }
        }

        ~MainEvent()
        {
            Dispose(false);
        }
    }

    public static class AddressMap
    {
        public static Dictionary<string, string> address(int index)
        {
            Dictionary<string, string> address = new Dictionary<string, string>();
            switch (index)
            {
                case 1:
                    address[PLCCommand.subCommandBit] = "100";
                    address[PLCCommand.subCommandSub] = "200";
                    address[PLCCommand.subCommandWord + "x"] = "100";
                    address[PLCCommand.subCommandWord + "y"] = "102";
                    break;
                case 2:
                    address[PLCCommand.subCommandBit] = "110";
                    address[PLCCommand.subCommandSub] = "210";
                    address[PLCCommand.subCommandWord + "x"] = "110";
                    address[PLCCommand.subCommandWord + "y"] = "112";
                    break;
                case 3:
                    address[PLCCommand.subCommandBit] = "120";
                    address[PLCCommand.subCommandSub] = "220";
                    address[PLCCommand.subCommandWord + "x"] = "120";
                    address[PLCCommand.subCommandWord + "y"] = "122";
                    break;
                case 4:
                    address[PLCCommand.subCommandBit] = "130";
                    address[PLCCommand.subCommandSub] = "230";
                    address[PLCCommand.subCommandWord + "x"] = "130";
                    address[PLCCommand.subCommandWord + "y"] = "132";
                    break;
            }

            return address;
        }
    }

}
