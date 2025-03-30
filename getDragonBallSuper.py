import os
import shutil
import requests
from bs4 import BeautifulSoup
from zipfile import ZipFile
import sys

if len(sys.argv) > 1:
    try:
        starting_volume = int(sys.argv[1])
    except ValueError:
        raise ValueError("The first argument must be an integer representing the starting volume.")
else:
    raise ValueError("A starting volume must be provided as the first argument.")

working_dir = os.path.join(os.environ['USERPROFILE'], 'Downloads', 'getDragonBallSuper')
download_dir_base = os.path.join(working_dir, 'download')
cbr_dir = os.path.join(working_dir, 'cbr')

final_dir = os.path.join(os.environ['USERPROFILE'], 'OneDrive', 'eComics (cbr,cbz)','Dragon Ball Super ESP')

# Creating working directory
if not os.path.exists(working_dir):
    os.makedirs(working_dir)

# Creating download directory
if os.path.exists(download_dir_base):
    shutil.rmtree(download_dir_base)
os.makedirs(download_dir_base)

# Creating cbr directory
if os.path.exists(cbr_dir):
    shutil.rmtree(cbr_dir)
os.makedirs(cbr_dir)

# Creating final directory
if not os.path.exists(final_dir):
    os.makedirs(final_dir)
    
try:
    volume = max(starting_volume, 1)

    while True:
        base_name = f"dbs {str(volume).zfill(3)}"
        downloadDirVol = os.path.join(download_dir_base, base_name)

        if 0 < volume <= 58:
            url = f"https://planetadragonball.com/dragon-ball-super-manga-{volume}-espanol/"
        else:
            url = f"https://radardeldragon.com/dragon-ball-super-manga-{volume}-espanol/"

        # Getting HTML
        print(f"getting html from {url.ljust(15)}")
        response = requests.get(url)
        html = response.text

        soup = BeautifulSoup(html, 'html.parser')

        imgs = []
        # Extracting image nodes
        if not imgs:
            try:
                imgs = soup.select("figure.wp-block-image img")
            except Exception:
                imgs = []

        if not imgs:
            try:
                imgs = soup.select("div.entry-content.clear img")
            except Exception:
                imgs = []
                
        if not imgs:
            raise RuntimeError(f"No images found for volume {volume}. Exiting.")

        #print(f"{imgs}")

        # Create image folder
        os.makedirs(downloadDirVol)

        # Download images
        for i, img in enumerate(imgs):
            
            if volume <= 58:
                img_src = img.get("data-src")
            else:
                img_src = img.get("src")
            
            img_name_with_path = os.path.join(downloadDirVol, f"img_{str(i).zfill(3)}.jpeg")

            print(f"getting {img_name_with_path.ljust(15)}")
            #print(f"[DBG] {img_src}")
            with requests.get(img_src, stream=True) as img_response:
                with open(img_name_with_path, 'wb') as img_file:
                    shutil.copyfileobj(img_response.raw, img_file)

        # create cbr name var
        cbr_name = f"{base_name}.cbr"

        # Compress directory
        cbr_path = os.path.join(cbr_dir, cbr_name)
        print(f"creating {cbr_path.ljust(15)}")
        with ZipFile(cbr_path, 'w') as zipf:
            for root, _, files in os.walk(downloadDirVol):
                for file in files:
                    file_path = os.path.join(root, file)
                    arcname = os.path.relpath(file_path, downloadDirVol)
                    zipf.write(file_path, arcname)

        # Move cbr to final directory
        final_path = os.path.join(final_dir, cbr_name)
        print(f"moving {cbr_path.ljust(15)} to {final_path.ljust(15)}")
        shutil.move(cbr_path, final_path)

        volume += 1

except requests.exceptions.RequestException as ex:
    print(ex)
finally:
    print("This is all")