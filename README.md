
# Sitecore Log Downloader (SCLD)
Sitecore Log Downloader (SCLD) simplifies downloading logs from your Azure PaaS instance. 

Locating log file on Scaled Azure PaaS instance [needs multiple steps](https://sitecorebasics.wordpress.com/2021/07/19/identify-current-sitecore-log-file-on-azure-paas-scaled-instance/). SCLD simplifies overall process and helps you download log files quickly. So, you can focus on fixing your main issue rather than trying to identify Instance and locate log file based on that. You even don't need Azure Portal access to download log files!

**Quick Demo:**![SCLD Demo](https://sitecorebasics.files.wordpress.com/2021/10/scld-v1.gif)

**Key Features:**
- Simplifies log download process
- Secure as you need to specify secure key
- Can be deployed and deleted without App Pool Restart
- Allows you to download multiple files in a single zip

**How to use?**
- Download files from main branch
- Change your Auth Key (You can use https://www.uuidgenerator.net/) and update **SCLD.aspx.cs**
- Deploy it on your target Sitecore instance
- Access https://HOSTNAME/DIRECTORY/SCLD.aspx?authkey=YOURSECUREKEY

**Quick Notes:**
- This tool should support PaaS, IaaS environments. But I haven't tested it.
- Please report bug(s) using Github
