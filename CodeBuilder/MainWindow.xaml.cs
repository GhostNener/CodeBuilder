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

namespace CodeBuilder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Settings settings = new Settings();

        static string modelstr;
        static string dalstr;

        private DataTable ExecuteDataTable(string sql)
        {
            //如果连接字符串填错就报错
            try
            {
                using (new SqlConnection(tbConnStr.Text)) { }
            }
            catch (Exception ex)
            {
                tbModel.Text = "连接字符串填写错误，请检查。错误信息：";
                tbDAL.Text = ex.Message;
                ConnectSqlFailed();
                return null;
            }

            using (SqlConnection conn = new SqlConnection(tbConnStr.Text))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    DataSet ds = new DataSet();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.FillSchema(ds, SchemaType.Source);
                        adapter.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
        }

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
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            GetMainView();
        }

        private void GetMainView()
        {
            //获取数据库的所有表的表名
            DataTable dt = null;
            try
            {
                dt = ExecuteDataTable("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
            }
            catch (SqlException sqlex)
            {
                ConnectSqlFailed();
                tbModel.Text = "数据库连接出错，错误信息：";
                tbDAL.Text = sqlex.Message;

                return;
            }
            if (dt == null) return;

            //把获取到的表名填充到下拉框内
            string[] tables = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                tables[i] = (string)row["TABLE_NAME"];
            }
            cbTables.ItemsSource = tables;
            cbTables.SelectedIndex = 0;
            cbDAL.SelectedIndex = 0;
            cbTables.IsEnabled = true;
            cbDAL.IsEnabled = true;
            btnGenerateCode.IsEnabled = true;
            btnConnect.IsEnabled = false;
            //tbConnStr.IsReadOnly = true;
            btnGenerateCode.IsDefault = true;
            btnExportALL.IsEnabled = true;
            tbModel.Text = "数据库连接成功\n\n----------------------------------------\n双击复制Model";
            tbDAL.Text = "数据库连接成功\n\n----------------------------------------\n双击复制DAL";
            //保存用户更新的信息
            settings.strConn = tbConnStr.Text;
            settings.strNamespace = tbNamespace.Text;
            settings.Save();
        }

        private void ConnectSqlFailed()
        {
            btnExport.IsEnabled = false;
            btnExportALL.IsEnabled = false;
            btnGenerateCode.IsEnabled = false;
            cbTables.ItemsSource = null;
            cbTables.IsEnabled = false;
            cbDAL.ItemsSource = null;
            cbDAL.IsEnabled = false;
            cbDAL.SelectedIndex = -1;
            tbConnStr.Focus();
        }

        private void btnGenerateCode_Click(object sender, RoutedEventArgs e)
        {
            string tableName = (string)cbTables.SelectedItem;
            CreateCode(tableName);
        }

        private void CreateCode(string tableName)
        {
            if (tableName == null)
            {
                tbDAL.Text = tbModel.Text = "请选择要生成的表";
                return;
            }

            else
            {
                DataTable dt = ExecuteDataTable("select top 0 * from [" + tableName + "]");
                CreateCodeHelper helper = new CreateCodeHelper();
                string strDAL = cbDAL.SelectedItem.ToString();
                StringBuilder sb = new StringBuilder();

                if (!tbNamespace.Text.Equals("命名空间") && tbNamespace.Text.Trim().Length > 0)
                {
                    modelstr = helper.CreateModelCode(tableName, dt, tbNamespace.Text).ToString();

                    if (strDAL.Equals("All DAL"))
                    {
                        dalstr = helper.CreateDALCode(tableName, dt, tbNamespace.Text).ToString();
                        tbDAL.Text = dalstr;
                    }
                    else
                    {
                        switch (strDAL)
                        {
                            case "All DAL":
                                {
                                    dalstr = helper.CreateDALCode(tableName, dt).ToString();
                                    break;
                                }
                            case "ListAll()":
                                {
                                    helper.CreateListAll(tableName, dt, sb, "");
                                    dalstr = sb.ToString();
                                    break;
                                }
                            case "DeleteById()":
                                {
                                    helper.CreateDeleteById(tableName, dt, sb, "");
                                    dalstr = sb.ToString();
                                    break;
                                }
                            case "GetById()":
                                {
                                    helper.CreateGetById(tableName, dt, sb, "");
                                    dalstr = sb.ToString();
                                    break;
                                }
                            case "Insert()":
                                {
                                    helper.CreateInsert(tableName, dt, sb, "");
                                    dalstr = sb.ToString();
                                    break;
                                }
                            case "Update()":
                                {
                                    helper.CreateUpdate(tableName, dt, sb, "");
                                    dalstr = sb.ToString();
                                    break;
                                }
                            case "ListByWhere()":
                                {
                                    helper.CreateListByWhere(tableName, sb, "");
                                    dalstr = sb.ToString();
                                    break;
                                }
                            default:
                                {
                                    dalstr = sb.ToString();
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    modelstr = helper.CreateModelCode(tableName, dt).ToString();

                    switch (strDAL)
                    {
                        case "All DAL":
                            {
                                dalstr = helper.CreateDALCode(tableName, dt).ToString();
                                break;
                            }
                        case "ListAll()":
                            {
                                helper.CreateListAll(tableName, dt, sb, "");
                                dalstr = sb.ToString();
                                break;
                            }
                        case "DeleteById()":
                            {
                                helper.CreateDeleteById(tableName, dt, sb, "");
                                dalstr = sb.ToString();
                                break;
                            }
                        case "GetById()":
                            {
                                helper.CreateGetById(tableName, dt, sb, "");
                                dalstr = sb.ToString();
                                break;
                            }
                        case "Insert()":
                            {
                                helper.CreateInsert(tableName, dt, sb, "");
                                dalstr = sb.ToString();
                                break;
                            }
                        case "Update()":
                            {
                                helper.CreateUpdate(tableName, dt, sb, "");
                                dalstr = sb.ToString();
                                break;
                            }
                        case "ListByWhere()":
                            {
                                helper.CreateListByWhere(tableName, sb, "");
                                dalstr = sb.ToString();
                                break;
                            }
                        default:
                            {
                                dalstr = sb.ToString();
                                break;
                            }
                    }
                    tbDAL.Text = dalstr;
                    tbModel.Text = modelstr;
                }
                tbDAL.Text = dalstr;
                tbModel.Text = modelstr;
            }
            btnExport.IsEnabled = true;
            btnGenerateCode.IsDefault = false;
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
            File.WriteAllText(fileModel, modelstr, Encoding.UTF8);
            tbModel.Text = "保存Model完成\n路径：" + fileModel + "\n----------------------------------------\n\n" + tbModel.Text;
            File.WriteAllText(fileDAL, dalstr, Encoding.UTF8);
            tbDAL.Text = "保存Model完成\n路径：" + fileDAL + "\n----------------------------------------\n\n" + tbDAL.Text;

        }
        /// <summary>
        /// 导出helper
        /// </summary>
        private void ExportHelper()
        {

            string fileHelper = settings.strPath + "Helper" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(fileHelper))
            {
                Directory.CreateDirectory(fileHelper);
            }
            File.WriteAllText(fileHelper + "SqlHelper.cs", settings.SqlHelper, Encoding.UTF8);
            File.WriteAllText(fileHelper + "GenericSQLGenerator.cs", settings.GenericSQLGeneratorHelper, Encoding.UTF8);
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
            this.IsEnabled = false;
            if (settings.strPath.IndexOf("双击此处选择文件导出路径") == 0)
            {
                if (SelectPath() == false)
                {
                    return;
                }
            }

            for (int i = 0; i < cbTables.Items.Count; i++)
            {
                CreateCode(cbTables.Items[i].ToString());
                Export(cbTables.Items[i].ToString());
            }
            ExportHelper();
            tbDAL.Text = "导出完成！";
            tbModel.Text = "导出完成！";
            System.Windows.Forms.MessageBox.Show("导出完成！");
            this.IsEnabled = true;

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
            System.Windows.MessageBox.Show("这是一个DotNet 代码生成器！\nPower by @QingWei-Li @GhostNener");
        }
    }
}
