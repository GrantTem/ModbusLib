/***************************************************************************   
** Company：ifancyit
** Author: linmiaoyong 
** Create Date: 2012-11-05
** Descriptions: Modbus的枚举
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusLib
{
    public class ModbusEnum
    {

        /// <summary>
        /// Modbus通信的当前状态
        /// </summary>
        public enum MBState
        {
            MB_ENOERR,                  /*!< no error. */
            MB_ENOREG,                  /*!< illegal register address. */
            MB_EINVAL,                  /*!< illegal argument. */
            MB_EPORTERR,                /*!< porting layer error. */
            MB_ENORES,                  /*!< insufficient resources. */
            MB_EIO,                     /*!< I/O error. */
            MB_EILLSTATE,               /*!< protocol stack in illegal state. */
            MB_ETIMEDOUT,               /*!< timeout error occurred. */
        }


        /// <summary>
        /// Modbus寄存器类型
        /// </summary>
        public enum ModbusRegisterType
        {
            Input,
            Holding,
        }

        /// <summary>
        /// Modbus操作
        /// </summary>
        public enum ModbusOperation
        {
            WriteMulit,
            Read,
        }


 
    }
}
