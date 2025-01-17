﻿using HotelManager.DAO;
using HotelManager.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace HotelManager
{
    public partial class fReceiveRoom : Form
    {
        List<int> ListIDCustomer=new List<int>();
        int IDBookRoom=-1;
        string RoomNameBR = "";
        DateTime dateCheckIn;
        public fReceiveRoom(int idBookRoom, string roomName)
        {
            txbIDBookRoom.Text = idBookRoom.ToString();
            IDBookRoom = idBookRoom;
            RoomNameBR = roomName;
            InitializeComponent();
            LoadData();
            ShowBookRoomInfo(IDBookRoom);
        }
        public fReceiveRoom()
        {
            InitializeComponent();
            LoadData();
          
        }
        public void LoadData()
        {
            LoadListRoomType();
            LoadReceiveRoomInfo();
        }
        public void LoadListRoomType()
        {
            List<RoomType> rooms = RoomTypeDAO.Instance.LoadListRoomType();
            cbRoomType.DataSource = rooms;
            cbRoomType.DisplayMember = "Name";
        }
        public void LoadEmptyRoom(int idRoomType)
        {
            DataTable rooms = RoomDAO.Instance.LoadFullRoom();

            //List<Room> roomList = new List<Room>();
            //foreach (DataRow row in rooms.Rows)
            //{
            //    Room room = new Room
            //    {
            //        Name = row["Name"].ToString()
            //    };
            //
            //    roomList.Add(room);
            //}

            cbRoom.DataSource = rooms;
            cbRoom.DisplayMember = "Name";
        }
        public bool IsIDBookRoomExists(int idBookRoom)
        {
            return BookRoomDAO.Instance.IsIDBookRoomExists(idBookRoom);
        }
        public void ShowBookRoomInfo(int idBookRoom)
        {
            DataRow dataRow = BookRoomDAO.Instance.ShowBookRoomInfo(idBookRoom);
            txbFullName.Text = dataRow["FullName"].ToString();
            txbIDCard.Text = dataRow["IDCard"].ToString();
            txbRoomName.Text = dataRow["RoomName"].ToString();
            txbRoomTypeName.Text = RoomTypeDAO.Instance.GetRoomTypeByIdBookRoom(idBookRoom).Name.ToString();
            txbDateCheckIn.Text = dataRow["DateCheckIn"].ToString().Split(' ')[0];
            dateCheckIn = (DateTime)dataRow["DateCheckIn"];
            txbDateCheckOut.Text = dataRow["DateCheckOut"].ToString().Split(' ')[0];
            txbAmountPeople.Text= dataRow["LimitPerson"].ToString();
            txbPrice.Text= dataRow["Price"].ToString();
        }
        public bool InsertReceiveRoom(int idBookRoom, int idRoom)
        {
            return ReceiveRoomDAO.Instance.InsertReceiveRoom(idBookRoom, idRoom);
        }
        public bool InsertReceiveRoomDetails(int idReceiveRoom, int idCustomerOther)
        {
            return ReceiveRoomDetailsDAO.Instance.InsertReceiveRoomDetails(idReceiveRoom, idCustomerOther);
        }
        public void LoadReceiveRoomInfo()
        {
            dataGridViewReceiveRoom.DataSource = ReceiveRoomDAO.Instance.LoadReceiveRoomInfo();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txbRoomTypeName.Text = (cbRoomType.SelectedItem as RoomType).Name;
            LoadEmptyRoom((cbRoomType.SelectedItem as RoomType).Id);
        }

        private void txbIDBookRoom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
            if (e.KeyChar == 13)
                btnSearch_Click(sender, null);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(txbIDBookRoom.Text!=string.Empty)
            {
                if (IsIDBookRoomExists(int.Parse(txbIDBookRoom.Text)))
                {
                    btnSearch.Tag = txbIDBookRoom.Text;
                    ShowBookRoomInfo(int.Parse(txbIDBookRoom.Text));
                }
                else
                   MessageBox.Show( "Mã đặt phòng không tồn tại.\nVui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            if (txbRoomName.Text != string.Empty && txbRoomTypeName.Text != string.Empty && txbFullName.Text != string.Empty && txbIDCard.Text != string.Empty && txbDateCheckIn.Text != string.Empty && txbDateCheckOut.Text != string.Empty && txbAmountPeople.Text != string.Empty && txbPrice.Text != string.Empty)
            {
                fAddCustomerInfo fAddCustomerInfo = new fAddCustomerInfo();
                fAddCustomerInfo.ShowDialog();
                this.Show();
            }
            else
                MessageBox.Show("Vui lòng nhập lại đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnReceiveRoom_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn nhận phòng không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (txbRoomName.Text != string.Empty && txbRoomTypeName.Text != string.Empty && txbFullName.Text != string.Empty && txbIDCard.Text != string.Empty && txbDateCheckIn.Text != string.Empty && txbDateCheckOut.Text != string.Empty && txbAmountPeople.Text != string.Empty && txbPrice.Text != string.Empty)
                {
                    if (dateCheckIn == DateTime.Now.Date)
                    {
                        int idBookRoom;
                        if (int.Parse(txbIDBookRoom.Text) != -1) idBookRoom = int.Parse(txbIDBookRoom.Text);
                        else idBookRoom = int.Parse(btnSearch.Tag.ToString());
                        //int idRoom = (cbRoom.SelectedItem as Room).Id;
                        int idRoom = RoomDAO.Instance.GetIdRoomByRoomName(txbRoomName.Text);
                        if (InsertReceiveRoom(idBookRoom, idRoom))
                        {
                            if (fAddCustomerInfo.ListIdCustomer != null)
                            {
                                foreach (int item in fAddCustomerInfo.ListIdCustomer)
                                {
                                    if (item != int.Parse(txbIDCard.Text))
                                        InsertReceiveRoomDetails(ReceiveRoomDAO.Instance.GetIDCurrent(), item);
                                }
                            }
                            MessageBox.Show("Nhận phòng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadEmptyRoom((cbRoomType.SelectedItem as RoomType).Id);
                        }
                        else
                            MessageBox.Show("Tạo phiếu nhận phòng thất bại.\nVui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show( "Ngày nhận phòng không hợp lệ.\nVui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClearData();
                    LoadReceiveRoomInfo();
                }
                else
                    MessageBox.Show("Vui lòng nhập lại đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ClearData()
        {
            txbFullName.Text = txbIDCard.Text = txbRoomTypeName.Text = txbDateCheckIn.Text = txbDateCheckOut.Text = txbAmountPeople.Text = txbPrice.Text = string.Empty;

        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnClose__Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            try
            {
                fReceiveRoomDetails f = new fReceiveRoomDetails((int)dataGridViewReceiveRoom.SelectedRows[0].Cells[0].Value);
                f.ShowDialog();
                Show();
                LoadReceiveRoomInfo();
            }
            catch
            {
                MessageBox.Show("Không có thông tin.");
            }
        }

        private void fReceiveRoom_Load(object sender, EventArgs e)
        {

        }
    }
}
