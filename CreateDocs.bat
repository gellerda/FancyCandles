echo OFF
echo;
echo "To work locally don't forget to comment the content of the index.md file."
echo;
RD /S /Q "docs"
cd docfx_project
echo ON

docfx -t D:\mydocs\WPF\FancyCandles\docfx_project\my_template
xcopy /s /i "_site" "..\docs"
docfx serve _site
pause
