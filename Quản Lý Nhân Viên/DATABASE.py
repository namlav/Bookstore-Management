import pyodbc

# --- CẤU HÌNH DATABASE ---
DB_CONFIG = {
    "DRIVER": "{ODBC Driver 17 for SQL Server}",
    "SERVER": "172.17.208.24",  # Thay bằng tên server của nhóm (VD: .\SQLEXPRESS)
    "DATABASE": "QLNHASACH_HQT",
    "TRUSTED_CONNECTION": "no",  # Dùng xác thực Windows
    # Nếu dùng user/pass SQL thì bỏ comment 2 dòng dưới và chỉnh 'TRUSTED_CONNECTION': 'no'
    'UID': 'quanly_login',
    'PWD': '456'
}


class Database:
    def __init__(self):
        self.conn_str = (
            f"DRIVER={DB_CONFIG['DRIVER']};"
            f"SERVER={DB_CONFIG['SERVER']};"
            f"DATABASE={DB_CONFIG['DATABASE']};"
            f"Trusted_Connection={DB_CONFIG['TRUSTED_CONNECTION']};"
        )
        # Nếu có user/pass
        if "UID" in DB_CONFIG:
            self.conn_str += f"UID={DB_CONFIG['UID']};PWD={DB_CONFIG['PWD']};"

    def execute_query(self, query, params=()):
        try:
            conn = pyodbc.connect(self.conn_str)
            cursor = conn.cursor()
            cursor.execute(query, params)
            conn.commit()
            return True, "Thành công"
        except Exception as e:
            return False, str(e)
        finally:
            try:
                conn.close()
            except:
                pass

    def fetch_data(self, query, params=()):
        try:
            conn = pyodbc.connect(self.conn_str)
            cursor = conn.cursor()
            cursor.execute(query, params)
            rows = cursor.fetchall()
            return rows
        except Exception as e:
            print(f"Lỗi kết nối: {e}")
            return []
