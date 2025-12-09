import { useEffect, useState } from 'react'
import { productService } from '../api/productService'
import { storageService } from '../api/storageService'
import './Dashboard.css'

const Dashboard = () => {
  const [stats, setStats] = useState({
    totalProducts: 0,
    totalStorage: 0,
    loading: true,
  })

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [productsRes, storageRes] = await Promise.all([
          productService.getAll({ pageSize: 1 }),
          storageService.getAll({ pageSize: 1 }),
        ])

        setStats({
          totalProducts: productsRes.totalItems || productsRes.TotalItems || 0,
          totalStorage: storageRes.totalItems || storageRes.TotalItems || 0,
          loading: false,
        })
      } catch (error) {
        console.error('Error fetching stats:', error)
        setStats({ ...stats, loading: false })
      }
    }

    fetchStats()
  }, [])

  if (stats.loading) {
    return <div className="loading">Đang tải...</div>
  }

  return (
    <div className="dashboard">
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon">📦</div>
          <div className="stat-content">
            <h3>Tổng sản phẩm</h3>
            <p className="stat-value">{stats.totalProducts}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">🏪</div>
          <div className="stat-content">
            <h3>Tổng kho</h3>
            <p className="stat-value">{stats.totalStorage}</p>
          </div>
        </div>
      </div>
      <div className="welcome-section">
        <h2>Chào mừng đến với hệ thống quản lý kho</h2>
        <p>Sử dụng menu bên trái để điều hướng đến các chức năng.</p>
      </div>
    </div>
  )
}

export default Dashboard

