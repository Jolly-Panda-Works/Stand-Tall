# بیلد خودکار اندروید با GitHub Actions

فایل workflow: `.github/workflows/android-release.yml`

## چه کاری انجام می‌دهد

با **هر push** (به هر برنچی):

1. شماره نسخه ساخته می‌شود: `major.minor` از `Project Settings > Player > Version` (الان `1.0`) و عدد آخر = شماره اجرای workflow. مثال: `1.0.37`.
2. `versionCode` اندروید = `100 + شماره اجرا` (همیشه بزرگ‌تر از قبلی، همان چیزی که Google Play و کافه‌بازار می‌خواهند).
3. پروژه با Unity `6000.6.0f1` (همان نسخهٔ `ProjectVersion.txt`) روی سرور لینوکس GitHub بیلد می‌شود و APK با keystore اصلی امضا می‌شود.
4. یک **Release** در صفحهٔ Releases ریپو ساخته می‌شود و APK به آن ضمیمه می‌شود:
   - برنچ `main` → Release معمولی با تگ `v1.0.37`
   - برنچ‌های دیگر → Pre-release با تگ `v1.0.37-<نام برنچ>`

push هایی که فقط فایل `.md` را عوض می‌کنند بیلد نمی‌گیرند. اگر push جدید روی همان برنچ بیاید، بیلد قبلی لغو می‌شود.

برای فایل `.aab` (Google Play): در تب **Actions** روی `Android Build & Release` بزنید، `Run workflow` و تیک `Also build an .aab` را بزنید.

## راه‌اندازی (یک بار)

۷ secret لازم است. ساده‌ترین راه، اجرای اسکریپت آماده از ریشهٔ پروژه است (GitHub CLI باید نصب و login باشد):

```powershell
powershell -ExecutionPolicy Bypass -File tools/ci/set-github-secrets.ps1
```

اسکریپت فایل لایسنس Unity (`C:\ProgramData\Unity\Unity_lic.ulf`) و keystore (`F:\Jolly Panda Studio\Mobile Games\com.jollypanda.standtall.keystore`) را خودش می‌خواند و ایمیل/رمز Unity، رمز keystore، نام alias و رمز alias را از شما می‌پرسد. رمزها جایی ذخیره نمی‌شوند.

یا دستی در `Settings > Secrets and variables > Actions`:

| Secret | مقدار |
|---|---|
| `UNITY_LICENSE` | کل محتوای فایل `Unity_lic.ulf` |
| `UNITY_EMAIL` | ایمیل اکانت Unity |
| `UNITY_PASSWORD` | رمز اکانت Unity |
| `ANDROID_KEYSTORE_BASE64` | محتوای keystore به base64 |
| `ANDROID_KEYSTORE_PASS` | رمز keystore |
| `ANDROID_KEYALIAS_NAME` | نام alias (در Player Settings > Publishing Settings) |
| `ANDROID_KEYALIAS_PASS` | رمز alias |

> اکانت Unity نباید ورود دو مرحله‌ای (2FA) داشته باشد، وگرنه فعال‌سازی لایسنس روی سرور شکست می‌خورد.

## نکته‌ها

- اولین بیلد طولانی است (حدود ۴۵ تا ۹۰ دقیقه، چون پوشهٔ Library از صفر ساخته می‌شود). بیلدهای بعدی از cache استفاده می‌کنند و سریع‌ترند.
- ریپو private است، پس دقیقه‌های Actions از سهمیهٔ اکانت کم می‌شود (پلن رایگان: ۲۰۰۰ دقیقه در ماه).
- برای عوض کردن عدد اصلی نسخه (مثلاً `1.1`)، فقط `Version` را در Player Settings عوض کنید؛ عدد آخر همچنان شماره اجراست.
- فایل `password.txt` در ریشهٔ ریپو commit شده است. اگر رمز keystore در آن است، آن را از git خارج کنید (`git rm --cached password.txt`) و به `.gitignore` اضافه کنید؛ رمزها فقط باید در Secrets باشند.
- برای تست سریع روی گوشی بدون CI: داخل Unity `Tools > LastFlag > Build > Android Test APK` (با کلید debug امضا می‌شود، خروجی در پوشهٔ `Builds/Android`).
