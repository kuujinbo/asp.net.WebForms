using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace kuujinbo.asp.net.WebForms
{
    public partial class encryptFile : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!IsPostBack)
            {
                ecKey.Text = encryptionKey.Text = Crypto.GenerateRijndaelManagedKey();
            }
        }

        protected void process(object sender, CommandEventArgs e)
        {
            if (Page.IsValid)
            {
                if (uploader.HasFile)
                {
                    HttpPostedFile f = uploader.PostedFile;
                    string encryptedExt = ".enc";
                    bool isEncrypted = actionType.SelectedIndex == 1;
                    Crypto crypt = new Crypto(encryptionKey.Text);
                    int length = f.ContentLength;
                    byte[] uploadBytes = new byte[length];
                    f.InputStream.Read(uploadBytes, 0, length);
                    byte[] downloadBytes = null;
                    bool hasError = false;
                    if (isEncrypted)
                    {
                        try
                        {
                            downloadBytes = crypt.Decrypt(uploadBytes);
                        }
                        catch (CryptographicException ce)
                        {
                            hasError = true;
                            errorMessage.Text =
                            "Cannot decrypt file; verify you have the correct password.";
                        }
                        catch { throw; }
                    }
                    else
                    {
                        downloadBytes = crypt.Encrypt(uploadBytes);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(f.FileName);
                    if (!isEncrypted)
                    {
                        fileName = fileName + Path.GetExtension(f.FileName) + encryptedExt;
                    }

                    if (!hasError)
                    {
                        Response.AddHeader(
                          "Content-disposition",
                          string.Format("attachment; filename={0}", fileName)
                        );
                        Response.BinaryWrite(downloadBytes);
                        Response.End();
                        return;
                    }
                }
                else
                {
                    errorMessage.Text = "Selected a file to upload.";
                }
            }
        }  


    }
}