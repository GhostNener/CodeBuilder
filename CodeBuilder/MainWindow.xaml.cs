using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CodeBuilder.Properties;
using System.Windows.Controls;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;

namespace CodeBuilder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int sqltype = 1;//数据库类型 1 sql server ；2 mysql
        public static string leftStr = "["; //左定界符
        public static string rightStr = "]";//右定界符
        delegate void HandelDelegate();//委托
        string cononstr = "";//链接字符串
        bool isRun = false;
        Settings settings = new Settings();
        StringBuilder modelstr=new StringBuilder ();
        StringBuilder dalstr=new StringBuilder();
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 执行sql返回dataable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private DataTable ExecuteDataTable(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                switch (sqltype)
                {
                    case 1:
                        using (SqlConnection conn = new SqlConnection(cononstr))
                        {
                            conn.Open();
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                                {
                                    adapter.FillSchema(ds, SchemaType.Source);
                                    adapter.Fill(ds);
                                }
                            }
                        }
                        break;
                    case 2:
                        using (MySqlConnection conn = new MySqlConnection(cononstr))
                        {
                            conn.Open();
                            using (MySqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = sql;
                                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                                {
                                    adapter.FillSchema(ds, SchemaType.Source);
                                    adapter.Fill(ds);
                                }
                            }
                        }
                        break;
                    default: break;
                }
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(delegate
                {
                    tbModel.Text = "连接字符串填写错误，请检查。\n错误信息：" + ex.Message + "\n" + ex.ToString();
                    tbDAL.Text = ex.Message + "\n" + ex.ToString();
                });
                ConnectSqlFailed();
                return null;
            }
            finally
            {
                this.Dispatcher.Invoke(delegate { btnConnect.IsEnabled = true; tbConnStr.IsEnabled = true; });
            }
        }
        /// <summary>
        /// 装载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbConnStr.Text = settings.strConn;
            tbNamespace.Text = settings.strNamespace;
            tbSelectPath.Text = settings.strPath;
            cbDAL.Items.Add("All DAL");
            cbDAL.Items.Add("Insert()");
            cbDAL.Items.Add("DeleteById()");
            cbDAL.Items.Add("Update()");
            cbDAL.Items.Add("GetById()");
            cbDAL.Items.Add("ListAll()");
            cbDAL.Items.Add("ListByWhere()");
            cbDAL.Items.Add("ListByPage()");
            sqlcom.Items.Add("SQL Server");
            sqlcom.Items.Add("MySql");
            sqlcom.SelectedIndex = 0;
            tbSelectPath.IsReadOnly = true;
        }
        /// <summary>
        /// 链接点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (isRun)
            {
                System.Windows.MessageBox.Show("正在执行其他操作，请等待其完成后再操作！");
                return;
            }
            if (sqlcom.SelectedIndex == 0)
            {
                sqltype = 1;
                leftStr = "[";
                rightStr = "]";

            }
            else
            {
                sqltype = 2;
                leftStr = "`";
                rightStr = "`";
            }
            CreateCodeHelper.sqltype = sqltype;
            CreateCodeHelper.leftStr = leftStr;
            CreateCodeHelper.rightStr = rightStr;
            cononstr = tbConnStr.Text;
            Thread th = new Thread(new ThreadStart(new HandelDelegate(GetMainView)));
            th.IsBackground = true;
            th.Start();
        }

        private void GetMainView()
        {
            isRun = true;
            this.Dispatcher.Invoke(delegate
            {
                tbConnStr.IsEnabled = false;
                btnConnect.IsEnabled = false;
                tbModel.Text = tbDAL.Text = "连接中...";
            });
            //获取数据库的所有表的表名
            DataTable dt = null;
            try
            {
                string gettable = sqltype == 2 ? "SHOW TABLES" : "Select Name FROM SysObjects Where XType IN ('U','V')orDER BY Name";
                dt = ExecuteDataTable(gettable);
                if (dt == null) return;
                //把获取到的表名填充到下拉框内
                string[] tables = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    tables[i] = (string)row[0];
                }
                this.Dispatcher.Invoke(delegate
                {
                    cbTables.ItemsSource = tables;
                    cbTables.SelectedIndex = 0;
                    cbDAL.SelectedIndex = 0;
                    cbTables.IsEnabled = true;
                    cbDAL.IsEnabled = true;
                    btnGenerateCode.IsEnabled = true;
                    btnConnect.IsEnabled = false;
                    btnGenerateCode.IsDefault = true;
                    btnExportALL.IsEnabled = true;
                    tbModel.Text = "数据库连接成功\n\n----------------------------------------\n双击复制Model";
                    tbDAL.Text = "数据库连接成功\n\n----------------------------------------\n双击复制DAL";
                    settings.strConn = tbConnStr.Text;
                    settings.strNamespace = tbNamespace.Text;
                    settings.Save();
                });
            }
            finally
            {
                this.Dispatcher.Invoke(delegate { btnConnect.IsEnabled = true; tbConnStr.IsEnabled = true; });
                isRun = false;
            }

        }
        /// <summary>
        /// 链接失败处理
        /// </summary>
        private void ConnectSqlFailed()
        {
            this.Dispatcher.Invoke(delegate
            {
                btnExport.IsEnabled = false;
                btnExportALL.IsEnabled = false;
                btnGenerateCode.IsEnabled = false;
                cbTables.IsEnabled = false; cbTables.ItemsSource = null;
                cbTables.IsEnabled = false; cbTables.ItemsSource = null; cbDAL.SelectedIndex = -1;
                tbConnStr.Focus();
            });
        }
        private void btnGenerateCode_Click(object sender, RoutedEventArgs e)
        {
            string tableName = (string)cbTables.SelectedItem;
            CreateCode(tableName);
        }
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="tableName"></param>
        private void CreateCode(string tableName)
        {
            if (tableName == null)
            {
                tbDAL.Text = tbModel.Text = "请选择要生成的表";
                return;
            }
            else
            {
                string getfiled = sqltype == 2 ? "select * from `" + tableName + "` LIMIT 0" : "select top 0 * from [" + tableName + "]";
                DataTable dt = ExecuteDataTable(getfiled);
                CreateCodeHelper helper = new CreateCodeHelper();
                string strDAL = "";
                this.Dispatcher.Invoke(delegate
                {
                    strDAL = cbDAL.SelectedItem.ToString();
                });
                StringBuilder sb = new StringBuilder();
                if (!tbNamespace.Text.Equals("命名空间") && tbNamespace.Text.Trim().Length > 0)
                {
                    modelstr = helper.CreateModelCode(tableName, dt, tbNamespace.Text);
                    if (strDAL.Equals("All DAL"))
                    {
                        dalstr = helper.CreateDALCode(tableName, dt, tbNamespace.Text);
                        tbDAL.Text = dalstr.ToString();
                    }
                    else
                    {
                        GetCode(tableName);
                    }
                }
                else
                {
                    modelstr = helper.CreateModelCode(tableName, dt);
                    GetCode(tableName);
                }
                tbDAL.Text = dalstr.ToString();
                tbModel.Text = modelstr.ToString();
            }
            btnExport.IsEnabled = true;
            btnGenerateCode.IsDefault = false;
        }

        private void GetCode(string tableName)
        {
            string getfiled = sqltype == 2 ? "select * from `" + tableName + "` LIMIT 0" : "select top 0 * from [" + tableName + "]";
            DataTable dt = ExecuteDataTable(getfiled);
            CreateCodeHelper helper = new CreateCodeHelper();
            string strDAL = cbDAL.SelectedItem.ToString();
            StringBuilder sb = new StringBuilder();
            switch (strDAL)
            {
                case "All DAL":
                    {
                        dalstr = helper.CreateDALCode(tableName, dt);
                        break;
                    }
                case "ListAll()":
                    {
                        helper.CreateListAll(tableName, dt, sb, "");
                        dalstr = sb;
                        break;
                    }
                case "DeleteById()":
                    {
                        helper.CreateDeleteById(tableName, dt, sb, "");
                        dalstr = sb;
                        break;
                    }
                case "GetById()":
                    {
                        helper.CreateGetById(tableName, dt, sb, "");
                        dalstr = sb;
                        break;
                    }
                case "Insert()":
                    {
                        helper.CreateInsert(tableName, dt, sb, "");
                        dalstr = sb;
                        break;
                    }
                case "Update()":
                    {
                        helper.CreateUpdate(tableName, dt, sb, "");
                        dalstr = sb;
                        break;
                    }
                case "ListByWhere()":
                    {
                        helper.CreateListByWhere(tableName, sb, "");
                        dalstr = sb;
                        break;
                    }
                case "ListByPage()":
                    {
                        helper.CreateListByPage(tableName, dt, sb, "");
                        dalstr = sb;
                        break;
                    }
                default:
                    {
                        dalstr = sb;
                        break;
                    }
            }
        }

        private void btnCopyModel(object sender, RoutedEventArgs e)
        {
            tbModel.Copy();
        }

        private void btnCopyDAL(object sender, RoutedEventArgs e)
        {
            tbDAL.Copy();
        }

        private void cbTables_GotFocus(object sender, RoutedEventArgs e)
        {
            btnGenerateCode.IsDefault = true;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string tableName = (string)cbTables.SelectedItem;
            CreateCode(tableName);
            Export(tableName);
            ExportHelper();
        }

        private void Export(string tablename)
        {
            if (settings.strPath.IndexOf("双击此处选择文件导出路径") == 0)
            {
                if (SelectPath() == false)
                {
                    return;
                }
            }
            if (tbSelectPath.Text.Length <= 0)
            {
                SelectPath();
                return;
            }
            if (tbModel.Text.IndexOf("复制完成") == 0)
            {
                tbModel.Text = tbModel.Text.Remove(tbModel.Text.IndexOf("复制完成"), "复制完成\n\n----------------------------------------\n\n".Length);
            }
            if (tbDAL.Text.IndexOf("复制完成") == 0)
            {
                tbDAL.Text = tbDAL.Text.Remove(tbDAL.Text.IndexOf("复制完成"), "复制完成\n\n----------------------------------------\n\n".Length);
            }
            //创建文件夹
            string fileDAL = settings.strPath + "DAL" + Path.DirectorySeparatorChar;
            string fileModel = settings.strPath + "Model" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(fileDAL))
            {
                Directory.CreateDirectory(fileDAL);
            }
            if (!Directory.Exists(fileModel))
            {
                Directory.CreateDirectory(fileModel);
            }
            fileDAL += tablename + "DAL.cs";
            fileModel += tablename + "Model.cs";
            File.WriteAllText(fileModel, modelstr.ToString(), Encoding.UTF8);
            tbModel.Text = "保存Model完成\n路径：" + fileModel + "\n----------------------------------------\n\n" + tbModel.Text;
            File.WriteAllText(fileDAL, dalstr.ToString(), Encoding.UTF8);
            tbDAL.Text = "保存Model完成\n路径：" + fileDAL + "\n----------------------------------------\n\n" + tbDAL.Text;

        }
        /// <summary>
        /// 导出helper
        /// </summary>
        private void ExportHelper()
        {
            string helper = sqltype == 2 ? "MySql" : "Sql";
            string fileHelper = settings.strPath + "Helper" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(fileHelper))
            {
                Directory.CreateDirectory(fileHelper);
            }
            File.WriteAllText(fileHelper + helper + "Helper.cs", sqltype == 2 ? settings.MySqlHelper : settings.SqlHelper, Encoding.UTF8);
            File.WriteAllText(fileHelper + "GenericSQLGenerator.cs", sqltype == 2 ? settings.GenericMySQLGeneratorHelper : settings.GenericSQLGeneratorHelper, Encoding.UTF8);
        }

        private void tbNamespace_LostFocus(object sender, RoutedEventArgs e)
        {
            tbNamespace.PreviewMouseDown += new MouseButtonEventHandler(tbNamespace_PreviewMouseDown);
        }

        private void tbNamespace_GotFocus(object sender, RoutedEventArgs e)
        {
            tbNamespace.SelectAll();
            tbNamespace.PreviewMouseDown -= new MouseButtonEventHandler(tbNamespace_PreviewMouseDown);
        }

        private void tbNamespace_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbNamespace.Focus();
            btnGenerateCode.IsDefault = true;
            e.Handled = true;
        }

        private void tbSelectPath_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectPath();
        }

        private bool SelectPath()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowDialog();
                if (fbd.SelectedPath != string.Empty)
                {
                    tbSelectPath.Text = fbd.SelectedPath + Path.DirectorySeparatorChar;
                    settings.strPath = fbd.SelectedPath + Path.DirectorySeparatorChar;
                    settings.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox tb = sender as System.Windows.Controls.TextBox;
            if (tb.Text.IndexOf("复制完成") != 0)
            {
                System.Windows.Clipboard.SetText(tb.Text);
                tb.Text = "复制完成\n\n----------------------------------------\n\n" + tb.Text;
            }
            else
            {
                tb.Text = tb.Text.Remove(tb.Text.IndexOf("复制完成"), "复制完成\n\n----------------------------------------\n\n".Length);
                System.Windows.Clipboard.SetText(tb.Text);
                tb.Text = "复制完成\n\n----------------------------------------\n\n" + tb.Text;
            }
        }

        private void btnExportALL_Click(object sender, RoutedEventArgs e)
        {
            if (isRun)
            {
                System.Windows.MessageBox.Show("正在执行其他操作，请等待其完成后再操作！");
                return;
            }
            Thread thExportAll = new Thread(new ThreadStart(new HandelDelegate(ExportAllCode)));
            thExportAll.IsBackground = true;
            thExportAll.Start();
        }

        private void ExportAllCode()
        {
            isRun = true;
            this.Dispatcher.Invoke(delegate
            {
                this.IsEnabled = false;
                if (settings.strPath.IndexOf("双击此处选择文件导出路径") == 0)
                {
                    if (SelectPath() == false)
                    {
                        return;
                    }
                }
            });
            for (int i = 0; i < cbTables.Items.Count; i++)
            {
                this.Dispatcher.Invoke(delegate
                {
                    tbDAL.Text = tbModel.Text = "导出中...";
                    CreateCode(cbTables.Items[i].ToString());
                    Export(cbTables.Items[i].ToString());
                });

            }
            ExportHelper();
            this.Dispatcher.Invoke(delegate
            {
                tbDAL.Text = tbModel.Text = "导出完成！";
                this.IsEnabled = true;
            });
            isRun = false;
        }
        private void tbConnStr_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            btnConnect.IsEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            GC.Collect();
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("这是一个DotNet 代码生成器！\nPower by  @Nener @QingWei-Li");
        }

        private void sqlcom_GotFocus(object sender, RoutedEventArgs e)
        {
            btnConnect.IsEnabled = true;
        }
    }
}
