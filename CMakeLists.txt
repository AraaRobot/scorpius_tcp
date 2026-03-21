cmake_minimum_required(VERSION 3.8)
project(ros_tcp_endpoint)

find_package(catkin REQUIRED COMPONENTS
  rospy
  std_msgs
)

catkin_python_setup()

catkin_package(CATKIN_DEPENDS message_runtime )
