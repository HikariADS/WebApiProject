import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './contexts/AuthContext'
import { ToastProvider } from './contexts/ToastContext'
import PrivateRoute from './components/PrivateRoute'
import Layout from './components/Layout'
import PublicLayout from './components/PublicLayout'
import Login from './pages/Login'
import Register from './pages/Register'
import VerifyEmail from './pages/VerifyEmail'
import ForgotPassword from './pages/ForgotPassword'
import ResetPassword from './pages/ResetPassword'
import Dashboard from './pages/Dashboard'
import Products from './pages/Products'
import ProductTypes from './pages/ProductTypes'
import Storage from './pages/Storage'
import StorageTypes from './pages/StorageTypes'
import Users from './pages/Users'
import Reports from './pages/Reports'
import News from './pages/News'
import Profile from './pages/Profile'

function App() {
  return (
    <AuthProvider>
      <ToastProvider>
        <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/verify-email" element={<VerifyEmail />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          {/* Trang tin tức công khai - không cần login */}
          <Route 
            path="/" 
            element={
              <PublicLayout>
                <News />
              </PublicLayout>
            } 
          />
          <Route 
            path="/news" 
            element={
              <PublicLayout>
                <News />
              </PublicLayout>
            } 
          />
          <Route
            path="/*"
            element={
              <PrivateRoute>
                <Layout>
                  <Routes>
                    <Route path="/dashboard" element={<Dashboard />} />
                    <Route path="/products" element={<Products />} />
                    <Route path="/product-types" element={<ProductTypes />} />
                    <Route path="/storage" element={<Storage />} />
                    <Route path="/storage-types" element={<StorageTypes />} />
                    <Route path="/users" element={<Users />} />
                    <Route path="/reports" element={<Reports />} />
                    <Route path="/profile" element={<Profile />} />
                    <Route path="*" element={<Navigate to="/" replace />} />
                  </Routes>
                </Layout>
              </PrivateRoute>
            }
          />
        </Routes>
      </Router>
      </ToastProvider>
    </AuthProvider>
  )
}

export default App

