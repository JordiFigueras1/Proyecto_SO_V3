using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);

                switch (codigo)
                {
                    case 1:  //Respuesta del registro

                        if (Convert.ToInt32(trozos[1]) == 1)
                        {
                            MessageBox.Show("Registro ok");
                        }
                        else
                        {
                            MessageBox.Show("Registro fallido");
                        }
                        nombre_registro.Clear();
                        password_registro.Clear();
                        break;

                    case 2: //Respuesta del login

                        if (Convert.ToInt32(trozos[1]) == 2)
                        {
                            MessageBox.Show("login ok");
                        }
                        else
                        {
                            MessageBox.Show("login fallido");
                        }
                        nombre_login.Clear();
                        password_login.Clear();
                        break;

                    case 3: //Respuesta de cuantas partidas ha ganado un jugador

                        if (Convert.ToInt32(trozos[1]) == 3)
                        {
                            MessageBox.Show("el jugador ha ganado:" + Convert.ToInt32(trozos[2]) + "partidas");
                        }
                        else
                        {
                            MessageBox.Show("peticion fallida");
                        }
                        textBox2.Clear();
                        break;

                    case 4: //Respuesta de cuantas partidas han jugado entre dos jugadores

                        if (Convert.ToInt32(trozos[1]) == 4)
                        {
                            MessageBox.Show("han jugado:" + Convert.ToInt32(trozos[2]) + " partidas");
                        }
                        else
                        {
                            MessageBox.Show("peticion fallida");
                        }
                        textBox1.Clear();
                        textBox3.Clear();
                        break;

                    case 5: //Respuesta a cuantos jugadores hay registrados

                        if (Convert.ToInt32(trozos[1]) == 5)
                        {
                            MessageBox.Show("hay:" + Convert.ToInt32(trozos[2]) + "jugadores registrados");
                        }
                        else
                        {
                            MessageBox.Show("peticion fallida");
                        }
                        break;

                    case 6: //Enviamos la lista a todos los usuarios conectados

                        if (Convert.ToInt32(trozos[1]) == 6)
                        {
                            int f = Convert.ToInt32(trozos[2]);
                            int i = 0;
                            if (dataGridView1.Rows.Count != 0)
                                dataGridView1.Rows.Clear();
                            while (i < f)
                            {
                                dataGridView1.Rows.Add(trozos[i + 3]);
                                i++;
                            }
                        }
                        else
                        {
                            MessageBox.Show("peticion fallida");
                        }
                        break;

                }

            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            // Envia el nombre y el password del registro con el código 1 y separado por /
            string mensaje = "1/" + Convert.ToString(nombre_registro.Text) + "/" + Convert.ToString(password_registro.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }


        private void Button3_Click(object sender, EventArgs e)
        {
            // Se terminó el servicio. 
            // Nos desconectamos
            this.BackColor = Color.Gray;
            dataGridView1.Rows.Clear();
            string mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            server.Shutdown(SocketShutdown.Both);
            server.Close();

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9070);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
            }

            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void Button_login_Click(object sender, EventArgs e)
        {

            // Envia el nombre y el password del login con el código 2 y separado por /
            string mensaje = "2/" + Convert.ToString(nombre_login.Text) + "/" + Convert.ToString(password_login.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string mensaje = "3/" + textBox2.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
                     
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string mensaje = "4/" + Convert.ToString(textBox1.Text) + "/" + Convert.ToString(textBox3.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
           
        }



        private void button5_Click(object sender, EventArgs e)
        {
            string mensaje = "5/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);


         
        }

    }
}
