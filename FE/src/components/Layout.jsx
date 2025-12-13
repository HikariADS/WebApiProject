import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useState, useEffect } from 'react'
import { useAuth } from '../contexts/AuthContext'
import './Layout.css'

const Layout = ({ children }) => {
  const location = useLocation()
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const toggleSidebar = () => {
    setSidebarOpen(!sidebarOpen)
  }

  const closeSidebar = () => {
    setSidebarOpen(false)
  }

  // Close sidebar when route changes on mobile
  useEffect(() => {
    closeSidebar()
  }, [location.pathname])

  const menuItems = [
    { path: '/', label: 'Dashboard', icon: '📊' },
    { path: '/products', label: 'Sản phẩm', icon: '📦' },
    { path: '/product-types', label: 'Loại sản phẩm', icon: '🏷️' },
    { path: '/storage', label: 'Kho', icon: '🏪' },
    { path: '/storage-types', label: 'Loại kho', icon: '📋' },
    // Chỉ hiển thị menu Users nếu user là Admin hoặc Manager
    ...(user?.roles?.includes('Admin') || user?.roles?.includes('Manager')
      ? [{ path: '/users', label: 'Người dùng', icon: '👥' }]
      : []),
  ]

  return (
    <div className="layout">
      <button className="menu-toggle" onClick={toggleSidebar} aria-label="Toggle menu">
        {sidebarOpen ? '✕' : '☰'}
      </button>
      <div 
        className={`sidebar-overlay ${sidebarOpen ? 'active' : ''}`}
        onClick={closeSidebar}
      />
      <aside className={`sidebar ${sidebarOpen ? 'open' : ''}`}>
        <div className="sidebar-header">
          <h2>Warehouse System</h2>
        </div>
        <nav className="sidebar-nav">
          {menuItems.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={`nav-item ${location.pathname === item.path ? 'active' : ''}`}
            >
              <span className="nav-icon">{item.icon}</span>
              <span className="nav-label">{item.label}</span>
            </Link>
          ))}
        </nav>
        <div className="sidebar-footer">
          <div className="user-info">
            <span>👤 {user?.userName || user?.email || 'User'}</span>
          </div>
          <button onClick={handleLogout} className="logout-btn">
            Đăng xuất
          </button>
        </div>
      </aside>
      <main className="main-content">
        <header className="main-header">
          <h1>
            {menuItems.find((item) => item.path === location.pathname)?.label || 'Dashboard'}
          </h1>
        </header>
        <div className="content">{children}</div>
      </main>
    </div>
  )
}

export default Layout

