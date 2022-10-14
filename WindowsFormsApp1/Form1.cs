using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Массив для биективного преобразования
        static byte[,] Pi =
        {
        { 1,7,14,13,0,5,8,3,4,15,10,6,9,12,11,2},
        { 8,14,2,5,6,9,1,12,15,4,11,0,13,10,3,7},
        { 5,13,15,6,9,2,12,10,11,7,8,1,4,3,14,0},
        { 7,15,5,10,8,1,6,13,0,9,3,14,11,4,2,12},
        { 12,8,2,1,13,4,15,6,7,0,10,5,3,14,9,11},
        { 11,3,5,8,2,15,10,13,14,1,7,4,12,9,6,0},
        { 6,8,2,3,9,10,5,12,1,14,4,7,11,13,0,15},
        { 12,4,6,2,10,5,11,9,14,8,13,7,0,3,15,1}
        };

        // 256 битный ключ
        static byte[] test_key = 
        {
        0xff, 0xfe, 0xfd, 0xfc,
        0xfb, 0xfa, 0xf9, 0xf8,
        0xf7, 0xf6, 0xf5, 0xf4,
        0xf3, 0xf2, 0xf1, 0xf0,
        0x00, 0x11, 0x22, 0x33,
        0x44, 0x55, 0x66, 0x77,
        0x88, 0x99, 0xaa, 0xbb,
        0xcc, 0xdd, 0xee, 0xff
        };

        public Form1()
        {
            InitializeComponent();
        }

        public void Encrypt() // Функция для расшифрования автоматическая
        {
            if (textBox1.Text.Length != 0)
            {
                var encrypt_test_string = Text2Bytes(textBox1.Text);
                string str = "";
                for (int i = 0; i < encrypt_test_string.Count; i++)
                {
                    var roundkeys = GetRoundKeys(test_key);
                    var result = EncryptOrDecrypt(encrypt_test_string[i], roundkeys);
                    str += Bytes2Text(result);
                }
                textBox2.Text = str;
            }
        }

        public void Decrypt() // Функция для расшифрования автоматическая
        {
            if (textBox1.Text.Length != 0)
            {
                var encrypt_test_string = Text2Bytes(textBox1.Text);
                string str = "";
                for (int i = 0; i < encrypt_test_string.Count; i++)
                {
                    var roundkeys = GetRoundKeys(test_key);
                    Array.Reverse(roundkeys);
                    var result = EncryptOrDecrypt(encrypt_test_string[i], roundkeys);
                    str += Bytes2Text(result);
                }
                textBox2.Text = str;
            }
        }


        public static List<byte[]> Text2Bytes(string data) // Функция преобразования текста в массив кодов символов
        {
            List<byte[]> list = new List<byte[]>(); // Список, который хранит массивы из 8  элементов по 8 бит
            int n = data.Length;
            int nn = (int)(n / 8 + 1);
            // Дополняем пустыми символами строку до тех пор, пока количество ее символов нацело не будет делиться на 8
            for (int i = 0; i < (nn * 8 - n); i++) 
            {
                data += (char)0;
            }
            int u = 0;
            for (int i = 0; i < nn; i++)
            {
                byte[] bytess = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    bytess[j] = (byte)data[u];
                    u++;
                }
                list.Add(bytess);
            }
            return list;
        }

        public static string Bytes2Text(byte[] data) // Функция преобразования массива кодов символов в текст
        {
            string str = "";
            int n = data.Length;
            for(int i =0; i < n; i++)
            {
                str+= (char)data[i];
            }
            return str;
        }

        public static uint Bytes2Uint(byte[] data) // Функция преобразования массива байтов в Uint32 
        {
            uint result;
            result = data[0];
            result = (result << 8) + data[1];
            result = (result << 8) + data[2];
            result = (result << 8) + data[3];
            return result;
        }

        public static byte[] Uint2bytes(uint data) // Функция преобразования Uint32 в массив байтов
        {
            byte[] result = new byte[4];
            result[3] = (byte)data;
            result[2] = (byte)(data >> 8);
            result[1] = (byte)(data >> 16);
            result[0] = (byte)(data >> 24);
            return result;
        }
        public static uint[] GetRoundKeys(byte[] key) // Функция получения раундовых ключей
        {
            uint[] roundkeys = new uint[32];
            for (int i = 0, j = 0; i < 24; i++, j = j + 4)
            {
                if (i % 8 == 0)
                {
                    j = 0;
                }
                roundkeys[i] = (uint)(256 * 256 * 256 * key[j] + 256 * 256 * key[j + 1] + 256 * key[j + 2] + key[j + 3]);
                string str = Convert.ToString(roundkeys[i], 16);
            }
            for (int i = 31, j = 0; i >= 24; i--, j = j + 4)
            {
                roundkeys[i] = (uint)(256 * 256 * 256 * key[j] + 256 * 256 * key[j + 1] + 256 * key[j + 2] + key[j + 3]);
                string str = Convert.ToString(roundkeys[i], 16);
            }
            return roundkeys;
        }

        public static uint Leftdata(byte[] data) // Функция получения левой части 64 битного числа
        {
            uint leftdata;
            leftdata = (uint)(256 * 256 * 256 * data[0] + 256 * 256 * data[1] + 256 * data[2] + data[3]);
            return leftdata;
        }

        public static uint Rightdata(byte[] data) // Функция получения правой части 64 битного числа
        {
            uint rightdata;
            rightdata = (uint)(256 * 256 * 256 * data[4] + 256 * 256 * data[5] + 256 * data[6] + data[7]);
            return rightdata;
        }

        public static uint Plusmod32(uint rightdata, uint key) // Функция сложения чисел по модулю 2^32
        {
            uint result;
            var max = UInt32.MaxValue;
            ulong sum = rightdata + key;
            result = (uint)(sum % max);
            return result;
        }

        public static byte[] BiectivnoePreobrazov(byte[] data, byte[,] Pi) // Функция биективного преобразования
        {
            byte[] result = new byte[4];
            byte left4bits, right4bits;
            for (int i = 0; i < 4; i++)
            {
                left4bits = (byte)(data[i] >> 4);
                right4bits = (byte)(data[i] & 0x0f);
                left4bits = Pi[i * 2, left4bits];
                right4bits = Pi[i * 2 + 1, right4bits];
                result[i] = (byte)((left4bits << 4) | right4bits);
            }
            return result;
        }


        public static byte[] XOR(byte[] leftdata, byte[] rightdata) // Функция сложения чисел по модулю 2
        {
            byte[] result = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = (byte)(leftdata[i] ^ rightdata[i]);
            }
            return result;
        }

        public static byte[] EncryptOrDecrypt(byte[] encrypt_test_string, uint[] roundkeys) // Функция, которая шифрует и расшифровывает текст
        {
            for (int u = 0; u < 32; u++) // Делаем преобразование в цикле 32 раунда
            {
                var rightdata = Rightdata(encrypt_test_string); // Получение правых 4 байтов блока
                var leftdata = Leftdata(encrypt_test_string); // Получение левых 4 байтов блока
                var rightdata_plusmod = Plusmod32(rightdata, roundkeys[u]); // Сложение по модулю 2^32 (правой части и раундового ключа)
                var rightdata_plusmod_dividebytes = Uint2bytes(rightdata_plusmod); // Разделение 32 битного числа на блоки по одному байту в массив byte
                var rightdata_plusmod_dividebytes_biektivn = BiectivnoePreobrazov(rightdata_plusmod_dividebytes, Pi); // Биективное преобразование 
                var rightdata_plusmod_dividebytes_biektivn_uint32 = Bytes2Uint(rightdata_plusmod_dividebytes_biektivn); // Преобразование массива byte в число Uint32
                uint rightdata_plusmod_dividebytes_biektivn_uint32_11_shift = (rightdata_plusmod_dividebytes_biektivn_uint32 << 11) | (rightdata_plusmod_dividebytes_biektivn_uint32 >> 21); // Циклический сдвиг на 11 битов влево
                var rightdata_plusmod_dividebytes_biektivn_11_shift_bytes = Uint2bytes(rightdata_plusmod_dividebytes_biektivn_uint32_11_shift); // Преобразование из uint32 в массив byte последней итоговой правой части 
                var leftdata_bytes = Uint2bytes(leftdata); // Преобразование из uint32 в массив byte левой части
                var rightdata_plusmod_dividebytes_biektivn_11_shift_bytes_XOR = XOR(leftdata_bytes, rightdata_plusmod_dividebytes_biektivn_11_shift_bytes); // Сложение по модулю 2 правой и левой части
                var rightdata_bytes = Uint2bytes(rightdata); // Преобразование uint32 в массив byte неизмененной правой части
                if (u < 31) // Дальнейшее преобразование с 1 по 31 включительно раунд
                {
                    for (int i = 0; i < 4; i++) // Меняем местами левую и правую части
                    {
                        leftdata_bytes[i] = rightdata_bytes[i]; // Записываем в левую часть неизмененную правую часть
                        rightdata_bytes[i] = rightdata_plusmod_dividebytes_biektivn_11_shift_bytes_XOR[i]; // В правую часть записываем измененную правую часть после XOR
                    }
                    for (int i = 0; i < 4; i++) // Склеиваем левую и правую части в один массив byte
                    {
                        encrypt_test_string[i] = leftdata_bytes[i];
                        encrypt_test_string[i + 4] = rightdata_bytes[i];
                    }
                }
                else // Если 32 раунд, то не меняем местами левую и правую часть
                {
                    for (int i = 0; i < 4; i++) // Склеиваем левую и правую части в один массив byte
                    {
                        encrypt_test_string[i] = rightdata_plusmod_dividebytes_biektivn_11_shift_bytes_XOR[i];
                        encrypt_test_string[i + 4] = rightdata_bytes[i];
                    }
                }
            }
            return encrypt_test_string;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) // Событие, связанное при изменение поля ввода текста
        {
            if (!radioButton1.Checked && !radioButton2.Checked && textBox1.Text.Length!=0)
            {
                MessageBox.Show("Вы не выбрали режим!");
                textBox1.Text = "";
            }
            else
            {
                if (radioButton1.Checked) Encrypt();
                else Decrypt();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) // Событие связанное с переключением RadioButton
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
