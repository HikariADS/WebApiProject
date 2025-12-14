import { Link, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import './PublicLayout.css'

const PublicLayout = ({ children }) => {
  const location = useLocation()
  const { user, logout } = useAuth()

  const handleLogout = () => {
    logout()
  }

  return (
    <div className="public-layout">
      <header className="public-header">
        <div className="header-content">
          <Link to="/" className="logo">
            <h1>Warehouse System</h1>
          </Link>
          <nav className="header-nav">
            {user ? (
              <>
                <Link to="/dashboard" className="nav-link">
                  Dashboard
                </Link>
                <Link to="/profile" className="profile-link-header">
                  <span className="profile-icon">👤</span>
                  <span className="profile-name">{user?.userName || user?.email || 'User'}</span>
                </Link>
                <button onClick={handleLogout} className="logout-btn-header">
                  Đăng xuất
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="nav-link">
                  Đăng nhập
                </Link>
                <Link to="/register" className="nav-link btn-register">
                  Đăng ký
                </Link>
              </>
            )}
          </nav>
        </div>
      </header>
      <main className="public-main">
        {children}
      </main>
    </div>
  )
}

export default PublicLayout

