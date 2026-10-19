#!/usr/bin/env bash
# حذف فایل‌هایی که در این نسخه باید از پروژه پاک شوند.
# اجرا از ریشهٔ پروژهٔ Unity (پوشه‌ای که Assets در آن است):
#   bash delete_removed_files.sh
#
# توجه: در این تحویل هیچ فایلی حذف نشده است (فقط فایل اضافه/ویرایش شده)،
# پس لیست پایین خالی است. اگر در قدم‌های بعدی فایلی حذف شد، مسیرش اینجا اضافه می‌شود.
# برای هر فایل، .meta آن هم حذف می‌شود.

FILES_TO_DELETE=(
  # "Assets/Scripts/Example/OldFile.cs"
)

if [ ! -d "Assets" ]; then
  echo "پوشهٔ Assets پیدا نشد. این اسکریپت را از ریشهٔ پروژه اجرا کنید."
  exit 1
fi

if [ ${#FILES_TO_DELETE[@]} -eq 0 ]; then
  echo "هیچ فایلی برای حذف وجود ندارد."
  exit 0
fi

for f in "${FILES_TO_DELETE[@]}"; do
  for target in "$f" "$f.meta"; do
    if [ -e "$target" ]; then
      rm -f "$target" && echo "حذف شد: $target"
    else
      echo "وجود نداشت (رد شد): $target"
    fi
  done
done
echo "تمام."
