/***************************************************************************   
** Company：ifancyit
** Author: linmiaoyong 
** Create Date: 2012-11-05
** Descriptions: Modbus的功能函数
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusLib
{

    /// <summary>
    /// modbus通信
    /// </summary>
    ///  <example>
    /// <code>
    /// 示例
    ///public bool CommonFunction_Set(ModTran.stRegister st)
    ///{
    ///try
    ///{
    ///
    ///ModTran.stRegister stTemp = st;
    ///
    ///   byte[] btSend = new byte[10];
    ///
    ///   ushort usQuantity = stTemp.usModbusQuantityofRegs;
    ///   btSend = ModTran.GreateSendInfo_Write_Multi(stTemp, st.usValue);
    ///
    ///   byte[] btBuffer = new byte[10];
    ///
    ///lock (Common.devMutex)
    ///{
    ///   if (!trans.SendAndReceive(btSend, out btBuffer))
    ///   {
    ///
    ///   Operation.strErrorInfo = "通信无响应！";
    ///   return false;
    ///}
    ///else
    ///{
    ///if (!ModTran.CheckResult(btBuffer, stTemp, ModbusEnum.ModbusOperation.WriteMulit))
    ///{
    ///   Operation.strErrorInfo = string.Format("返回信息无效!返回的结果为：{0}\r", Operation.DisPackage(btBuffer));
    ///   return false;
    ///}
    ///else
    ///{
    ///return true;
    ///}
    ///}
    /// }
    ///}
    ///catch (Exception ex)
    ///{
    ///   Operation.strErrorInfo = ex.ToString();
    ///   return false;
    ///}
    ///}
    ///</code>
    ///</example>
    public static class ModTran
    {
        /// <summary>
        /// 与其通信的从设备地址
        /// </summary>
        public static byte btSlaveDeviceAdd = 0x0A;


        /// <summary>
        /// modbus寄存器结构体
        /// </summary>
        public struct stRegister
        {
            public void IntStruct(ModbusEnum.ModbusRegisterType a, ushort b, ushort c)
            {
                emModusRegType = a;
                usModbusStartingAdd = b;
                usModbusQuantityofRegs = c;
            }
            public ModbusEnum.ModbusRegisterType emModusRegType;
            public ushort usModbusStartingAdd;
            public ushort usModbusQuantityofRegs;
            public ushort[] usValue;
        }


        /// <summary>
        /// 生成读寄存器的所要发的数据
        /// </summary>
        /// <param name="stReg"></param>
        /// <param name="usReadQuantity"></param>
        /// <returns></returns>
        public static byte[] GreateSendInfo_Read(stRegister stReg)
        {
            ushort usReadQuantity = stReg.usModbusQuantityofRegs;
            int iSendLength = 1 + 5 + 2;
            byte[] btSend = new byte[iSendLength];


            ushort usCRC = 0;


            byte btFunCode = 0;
            if (stReg.emModusRegType == ModbusEnum.ModbusRegisterType.Holding)
            {
                btFunCode = 3;
            }
            else
            {
                btFunCode = 4;
            }

            //设备地址
            btSend[0] = btSlaveDeviceAdd;

            //功能码  Function code 
            btSend[1] = btFunCode;

            //Starting Address  
            btSend[2] = (byte)((stReg.usModbusStartingAdd & 0xff00) >> 8);
            btSend[3] = (byte)((stReg.usModbusStartingAdd & 0x00ff));

            //Quantity of Registers
            btSend[4] = (byte)((usReadQuantity & 0xff00) >> 8);
            btSend[5] = (byte)((usReadQuantity & 0x00ff));

            //要发送数据

            usCRC = CRC.MBCRC16(btSend, (ushort)(iSendLength - 2));
            btSend[iSendLength - 2] = (byte)(usCRC & 0xff);
            btSend[iSendLength - 1] = (byte)((usCRC & 0xff00) >> 8);

            return btSend;  //要发的数据
        }


        /// <summary>
        /// 生成写多个寄存器所要发送的数据
        /// </summary>
        /// <param name="stReg"></param>
        /// <param name="btValue"></param>
        /// <returns></returns>
       
        public static byte[] GreateSendInfo_Write_Multi(stRegister stReg, ushort[] usValue)
        {
            ushort usQuantityOfReg = stReg.usModbusQuantityofRegs;
            byte[] btResult = new byte[7 + 2 * usQuantityOfReg + 2];


            ushort usCRC = 0;

            //设备地址
            btResult[0] = btSlaveDeviceAdd;

            //功能码  Function code 
            btResult[1] = 0x10;

            //Starting Address  
            btResult[2] = (byte)((stReg.usModbusStartingAdd & 0xff00) >> 8);
            btResult[3] = (byte)((stReg.usModbusStartingAdd & 0x00ff));

            //Quantity of Registers
            btResult[4] = (byte)((usQuantityOfReg & 0xff00) >> 8);
            btResult[5] = (byte)((usQuantityOfReg & 0x00ff));

            //Count
            btResult[6] = (byte)(usQuantityOfReg * 2);

            int iLoop = 0;
            for (iLoop = 0; iLoop < usValue.Length; iLoop++)
            {

                btResult[2 * iLoop + 7] = (byte)((usValue[iLoop] & 0xff00) >> 8);

                btResult[2 * iLoop + 8] = (byte)((usValue[iLoop] & 0x00ff));

            }

            //CRC
            usCRC = CRC.MBCRC16(btResult, (ushort)(btResult.Length - 2));

            btResult[btResult.Length - 2] = (byte)(usCRC & 0xff);
            btResult[btResult.Length - 1] = (byte)((usCRC & 0xff00) >> 8);

            return btResult;  //要发的数据
        }


        /// <summary>
        /// 生成写多个寄存器所要发送的数据
        /// </summary>
        /// <param name="stReg"></param>
        /// <param name="btValue"></param>
        /// <returns></returns>
        public static byte[] GreateSendInfo_Write_Multi(stRegister stReg, byte[] btValue)
        {
            ushort usQuantityOfReg = stReg.usModbusQuantityofRegs;
            byte[] btResult = new byte[7 + 2 * usQuantityOfReg + 2];


            ushort usCRC = 0;

            //设备地址
            btResult[0] = btSlaveDeviceAdd;

            //功能码  Function code 
            btResult[1] = 0x10;

            //Starting Address  
            btResult[2] = (byte)((stReg.usModbusStartingAdd & 0xff00) >> 8);
            btResult[3] = (byte)((stReg.usModbusStartingAdd & 0x00ff));

            //Quantity of Registers
            btResult[4] = (byte)((usQuantityOfReg & 0xff00) >> 8);
            btResult[5] = (byte)((usQuantityOfReg & 0x00ff));

            //Count
            btResult[6] = (byte)(usQuantityOfReg * 2);

            int iLoop = 0;
            for (iLoop = 0; iLoop < btValue.Length; iLoop++)
            {
                btResult[iLoop + 7] = btValue[iLoop];
            }
            if (btValue.Length % 2 == 1)  //补齐一位
            {
                btResult[iLoop + 7] = 0xff;
            }

            //CRC
            usCRC = CRC.MBCRC16(btResult, (ushort)(btResult.Length - 2));

            btResult[btResult.Length - 2] = (byte)(usCRC & 0xff);
            btResult[btResult.Length - 1] = (byte)((usCRC & 0xff00) >> 8);

            return btResult;  //要发的数据
        }


        /// <summary>
        /// modbus结果校验
        /// </summary>
        /// <param name="btResult"></param>
        /// <param name="stReg"></param>
        /// <param name="usQuantityOfReg"></param>
        /// <param name="oPt"></param>
        /// <returns></returns>
        public static bool CheckResult(byte[] btResult, stRegister stReg, ModbusEnum.ModbusOperation oPt)
        {

            ushort usQuantityOfReg = stReg.usModbusQuantityofRegs;
            if (oPt == ModbusEnum.ModbusOperation.WriteMulit)
            {

                const int iExpectLength = 8;

                if (iExpectLength > btResult.Length)  //结果长度不够
                {
                    return false;
                }
                byte[] btExpect = new byte[iExpectLength];
                ushort usCRC = 0;

                //设备地址
                btExpect[0] = btSlaveDeviceAdd;

                //功能码  Function code 
                btExpect[1] = 0x10;

                //Starting Address  
                btExpect[2] = (byte)((stReg.usModbusStartingAdd & 0xff00) >> 8);
                btExpect[3] = (byte)((stReg.usModbusStartingAdd & 0x00ff));

                //Quantity of Registers
                btExpect[4] = (byte)((usQuantityOfReg & 0xff00) >> 8);
                btExpect[5] = (byte)((usQuantityOfReg & 0x00ff));

                //CRC
                usCRC = CRC.MBCRC16(btResult, 6);

                btExpect[6] = (byte)(usCRC & 0xff);
                btExpect[7] = (byte)((usCRC & 0xff00) >> 8);


                //比较
                for (int iLoop = 0; iLoop < iExpectLength - 2; iLoop++)
                {
                    if (btResult[iLoop] != btExpect[iLoop])
                    {
                        return false;
                    }
                }

                return true;

            }
            else
            {

                int iExpectLength = 3 + 2 * usQuantityOfReg + 2;

                if (iExpectLength > btResult.Length)  //结果长度不够
                {
                    return false;
                }
                byte[] btExpect = new byte[iExpectLength];
                ushort usCRC = 0;


                byte btFunCode = 0;
                if (stReg.emModusRegType == ModbusEnum.ModbusRegisterType.Holding)
                {
                    btFunCode = 3;
                }
                else
                {
                    btFunCode = 4;
                }


                //设备地址
                btExpect[0] = btSlaveDeviceAdd;

                if (btResult[0] != btExpect[0])
                {
                    return false;
                }

                //功能码  Function code 
                btExpect[1] = btFunCode;

                if (btResult[1] != btExpect[1])
                {
                    return false;
                }

                //Byte count 
                btExpect[2] = (byte)(usQuantityOfReg * 2);

                if (btResult[2] != btExpect[2])
                {
                    return false;
                }


                //CRC
                usCRC = CRC.MBCRC16(btResult, (ushort)(iExpectLength - 2));
                btExpect[iExpectLength - 2] = (byte)(usCRC & 0xff);

                if (btResult[iExpectLength - 2] != btExpect[iExpectLength - 2])
                {
                    return false;
                }
                btExpect[iExpectLength - 1] = (byte)((usCRC & 0xff00) >> 8);


                if (btResult[iExpectLength - 1] != btExpect[iExpectLength - 1])
                {
                    return false;
                }

                return true;

            }
        }


        /// <summary>
        /// 获取结果,去掉协议的内容
        /// </summary>
        /// <param name="btResult"></param>
        /// <param name="stReg"></param>
        /// <param name="usQuantityOfReg"></param>
        /// <returns></returns>
        public static byte[] GetOutTheReadInfo(byte[] btResult, stRegister stReg)
        {

            ushort usQuantityOfReg = stReg.usModbusQuantityofRegs;
            byte[] btGetOut = new byte[2 * usQuantityOfReg];

            for (int iLoop = 0; iLoop < 2 * usQuantityOfReg; iLoop++)
            {
                btGetOut[iLoop] = btResult[iLoop + 3];
            }


            return btGetOut;
        }


    }
}
