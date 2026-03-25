from launch import LaunchDescription
from launch_ros.actions import Node


def generate_launch_description():
    return LaunchDescription(
        [
            Node(
                package="scorpius_tcp",
                executable="default_server_endpoint",
                emulate_tty=True,
                parameters=[{"ROS_IP": "127.0.0.1"}, {"ROS_TCP_PORT": 10000}],
            )
        ]
    )
