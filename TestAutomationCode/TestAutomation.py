import unittest
import os
from appium import webdriver
from time import sleep
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import NoSuchElementException

# opencv를 사용하기 위해 아래 모듈을 import합니다.
import cv2
import numpy as np
import time, datetime

class Matching():

    def detectimage(self, screenshotPath, detectImagePath):

        sourceimage = cv2.imread(screenshotPath, 0)
        template = cv2.imread(detectImagePath, 0)

        w, h = template.shape[::-1]

        method = eval('cv2.TM_CCOEFF')
        res = cv2.matchTemplate(sourceimage, template, method)
        min_val, max_val, min_loc, max_loc = cv2.minMaxLoc(res)

        print('max_val: %d' % max_val)

        top_left = max_loc
        bottom_right = (top_left[0] + w, top_left[1] + h)
        center = (top_left[0] + int(w/2), top_left[1] + int(h/2))

        color = (0, 0, 255)
        cv2.rectangle(sourceimage, top_left, bottom_right, color, thickness=8)

        detectshotPath = screenshotPath[:-4] + '-detect.png'
        cv2.imwrite(detectshotPath, sourceimage)

        return center

class SmithLoginTest(unittest.TestCase):

    def makeTS(self):
        return str(int(datetime.datetime.now().timestamp()))

    def strDatetime(self):
        return str(datetime.datetime.now().strftime("%Y%m%d%H%M"))

    def setUp(self):
        # Test App 경로
        app = os.path.join(os.path.dirname(__file__), 'C:\\Users\\iris2\\Desktop', 'smithlogin.apk')
        app = os.path.abspath(app)

        # Set up appium
        self.driver = webdriver.Remote(
            command_executor='http://127.0.0.1:4723/wd/hub',
            desired_capabilities={
                'app': app,
                'platformName': 'Android',
                'platformVersion': '5.0.0',
                'deviceName': 'Galaxy Note 3',
                'automationName': 'Appium',
                'newCommandTimeout': 500,
                'appPackage': 'com.smith.smithlogin',
                'appActivity': 'com.unity3d.player.UnityPlayerActivity',
                'udid': 'a1e35bd5'
            })

    def test_search_field(self):
        matching = Matching()
        # 스크린샷을 저장할 폴더를 생성합니다.
        test_folder_name = self.strDatetime()
        currentPath = '%s/' % os.getcwd()
        test_Directory = currentPath + test_folder_name + '/'

        if not os.path.exists(test_Directory):
            os.makedirs(test_Directory)

        driver = self.driver
        wait = WebDriverWait(driver, 20)

        sleep(50)

        # 'Init' 이미지 찾아z tap 한다.
        screenshotPath = test_Directory + '%s-screenshot.png' % self.makeTS()
        detectImagePath = currentPath + 'searchimages/InitButton.png'
        driver.save_screenshot(screenshotPath)

        InitButtonTouch = matching.detectimage(screenshotPath, detectImagePath)
        driver.tap([InitButtonTouch])

        # 30초 동안 기다립니다.
        sleep(30)


    def tearDown(self):
        self.driver.quit()


if __name__ == '__main__':
    suite = unittest.TestLoader().loadTestsFromTestCase(SmithLoginTest)
    unittest.TextTestRunner(verbosity=2).run(suite)