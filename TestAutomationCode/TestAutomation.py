import unittest
import os
from appium import webdriver
from time import sleep
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC


class TableSearchTest(unittest.TestCase):

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
                'appPackage': 'com.smith.smithlogin',
                'appActivity': 'com.unity3d.player.UnityPlayerActivity',
                'udid': 'a1e35bd5'
            })

    def test_search_field(self):
        driver = self.driver
        wait = WebDriverWait(driver, 20)

        sleep(30)
        # 테스트 시나리오에 따라 selenium 작성

    def tearDown(self):
        self.driver.quit()


if __name__ == '__main__':
    suite = unittest.TestLoader().loadTestsFromTestCase(TableSearchTest)
    unittest.TextTestRunner(verbosity=2).run(suite)